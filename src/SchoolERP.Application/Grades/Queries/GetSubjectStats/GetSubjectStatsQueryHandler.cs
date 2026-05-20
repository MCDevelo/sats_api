using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Grades.Queries.GetSubjectStats;

public class GetSubjectStatsQueryHandler : IRequestHandler<GetSubjectStatsQuery, ErrorOr<SubjectStatsResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSubjectStatsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SubjectStatsResult>> Handle(GetSubjectStatsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var subject = await _db.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.TenantId == tenantId, cancellationToken);

        if (subject is null)
            return Error.NotFound(description: "Materia no encontrada.");

        var section = await _db.Sections
            .AsNoTracking()
            .Include(s => s.GradeLevel)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var period = await _db.AcademicPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        var enrolledStudentIds = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var plans = await _db.EvaluationPlans
            .AsNoTracking()
            .Where(ep => ep.SubjectId == request.SubjectId && ep.AcademicPeriodId == request.AcademicPeriodId)
            .OrderBy(ep => ep.CreatedAt)
            .ToListAsync(cancellationToken);

        var planIds = plans.Select(p => p.Id).ToList();

        var allEntries = await _db.GradeEntries
            .AsNoTracking()
            .Where(ge =>
                ge.SubjectId == request.SubjectId &&
                ge.AcademicPeriodId == request.AcademicPeriodId &&
                planIds.Contains(ge.EvaluationPlanId) &&
                enrolledStudentIds.Contains(ge.StudentId))
            .ToListAsync(cancellationToken);

        // Compute weighted averages per student
        var studentAverages = enrolledStudentIds
            .Select(sid =>
            {
                var studentEntries = allEntries.Where(e => e.StudentId == sid).ToList();
                var scoredPlans = plans.Where(p => studentEntries.Any(e => e.EvaluationPlanId == p.Id)).ToList();
                if (!scoredPlans.Any()) return (StudentId: sid, Average: (decimal?)null);

                var usedWeight = scoredPlans.Sum(p => p.Weight);
                var avg = usedWeight > 0
                    ? scoredPlans.Sum(p => studentEntries.First(e => e.EvaluationPlanId == p.Id).Score * (p.Weight / usedWeight))
                    : (decimal?)null;

                return (StudentId: sid, Average: avg);
            })
            .ToList();

        var graded = studentAverages.Count(s => s.Average.HasValue);
        var scores = studentAverages.Where(s => s.Average.HasValue).Select(s => s.Average!.Value).ToList();

        decimal? classAverage = scores.Count > 0 ? Math.Round(scores.Average(), 2) : null;
        decimal? highest = scores.Count > 0 ? scores.Max() : null;
        decimal? lowest = scores.Count > 0 ? scores.Min() : null;
        var passing = scores.Count(s => s >= 60);
        var failing = scores.Count(s => s < 60);
        var passRate = graded > 0 ? Math.Round((decimal)passing / graded * 100, 1) : 0;

        // Score distribution (F/D/C/B/A)
        var total = enrolledStudentIds.Count;
        var distribution = new[]
        {
            BuildBucket("F (0-59)",   scores.Count(s => s < 60),           total),
            BuildBucket("D (60-69)",  scores.Count(s => s is >= 60 and < 70), total),
            BuildBucket("C (70-79)",  scores.Count(s => s is >= 70 and < 80), total),
            BuildBucket("B (80-89)",  scores.Count(s => s is >= 80 and < 90), total),
            BuildBucket("A (90-100)", scores.Count(s => s >= 90),           total),
        };

        // Per-evaluation stats
        var evalStats = plans.Select(plan =>
        {
            var planEntries = allEntries.Where(e => e.EvaluationPlanId == plan.Id).ToList();
            var planScores = planEntries.Select(e => e.Score).ToList();

            return new EvaluationStat(
                EvaluationName: plan.Name,
                Weight: plan.Weight,
                Average: planScores.Count > 0 ? Math.Round(planScores.Average(), 2) : null,
                Highest: planScores.Count > 0 ? planScores.Max() : null,
                Lowest: planScores.Count > 0 ? planScores.Min() : null,
                Graded: planScores.Count);
        }).ToList();

        return new SubjectStatsResult(
            SubjectName: subject.Name,
            SectionName: section.Name,
            PeriodName: period.Name,
            TotalStudents: total,
            Graded: graded,
            ClassAverage: classAverage,
            HighestScore: highest,
            LowestScore: lowest,
            Passing: passing,
            Failing: failing,
            PassRate: passRate,
            Distribution: distribution,
            EvaluationStats: evalStats);
    }

    private static ScoreDistribution BuildBucket(string range, int count, int total) =>
        new(range, count, total > 0 ? Math.Round((decimal)count / total * 100, 1) : 0);
}
