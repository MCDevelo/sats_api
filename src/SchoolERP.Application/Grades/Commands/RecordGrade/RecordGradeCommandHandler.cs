using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;
using GradeEntryEntity = SchoolERP.Domain.Entities.GradeEntry;

namespace SchoolERP.Application.Grades.Commands.RecordGrade;

public class RecordGradeCommandHandler : IRequestHandler<RecordGradeCommand, ErrorOr<RecordGradeResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RecordGradeCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<RecordGradeResult>> Handle(RecordGradeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var teacherId = _currentUser.UserId;

        var plan = await _db.EvaluationPlans
            .Include(ep => ep.Subject)
            .Include(ep => ep.AcademicPeriod)
            .FirstOrDefaultAsync(ep => ep.Id == request.EvaluationPlanId && ep.TenantId == tenantId, cancellationToken);

        if (plan is null)
            return Error.NotFound(description: "Plan de evaluación no encontrado.");

        if (plan.AcademicPeriod.GradesPublished)
            return Error.Conflict(description: "Las calificaciones de este período ya fueron publicadas y no pueden modificarse.");

        var studentIds = request.Entries.Select(e => e.StudentId).ToList();

        // Validate students are enrolled in a section that has this subject assigned
        var validStudentIds = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.Status == EnrollmentStatus.Active &&
                studentIds.Contains(e.StudentId) &&
                _db.TeacherAssignments.Any(ta =>
                    ta.SectionId == e.SectionId &&
                    ta.SubjectId == plan.SubjectId &&
                    ta.AcademicYearId == e.AcademicYearId &&
                    ta.IsActive))
            .Select(e => e.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var invalidStudents = studentIds.Except(validStudentIds).ToList();
        if (invalidStudents.Count > 0)
            return Error.Validation(description: $"Estudiante(s) no matriculado(s) en esta materia: {string.Join(", ", invalidStudents)}.");

        // Load existing grade entries for upsert
        var existingEntries = await _db.GradeEntries
            .Where(ge => ge.EvaluationPlanId == request.EvaluationPlanId && studentIds.Contains(ge.StudentId))
            .ToListAsync(cancellationToken);

        // Load student names
        var students = await _db.Students
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.FirstName, s.LastName })
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        var savedEntries = new List<GradeEntryEntity>();

        foreach (var entry in request.Entries)
        {
            var existing = existingEntries.FirstOrDefault(ge => ge.StudentId == entry.StudentId);

            if (existing is not null)
            {
                existing.Update(entry.Score, entry.Comments);
                savedEntries.Add(existing);
            }
            else
            {
                var gradeEntry = GradeEntryEntity.Create(
                    tenantId: tenantId,
                    studentId: entry.StudentId,
                    subjectId: plan.SubjectId,
                    academicPeriodId: plan.AcademicPeriodId,
                    evaluationPlanId: plan.Id,
                    score: entry.Score,
                    teacherId: teacherId,
                    comments: entry.Comments);

                _db.GradeEntries.Add(gradeEntry);
                savedEntries.Add(gradeEntry);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var resultEntries = savedEntries.Select(ge =>
        {
            students.TryGetValue(ge.StudentId, out var s);
            return new GradeEntryResult(
                GradeEntryId: ge.Id,
                StudentId: ge.StudentId,
                StudentFullName: s is not null ? $"{s.FirstName} {s.LastName}" : string.Empty,
                Score: ge.Score,
                LetterGrade: ge.GetLetterGrade(),
                IsPassing: ge.IsPassing(),
                Comments: ge.Comments);
        }).ToList();

        var avg = savedEntries.Count > 0 ? Math.Round(savedEntries.Average(e => e.Score), 2) : 0;

        return new RecordGradeResult(
            EvaluationPlanId: plan.Id,
            EvaluationName: plan.Name,
            TotalGraded: savedEntries.Count,
            AverageScore: avg,
            Passing: savedEntries.Count(e => e.IsPassing()),
            Failing: savedEntries.Count(e => !e.IsPassing()),
            Entries: resultEntries);
    }
}
