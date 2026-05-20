using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Grades.Commands.CreateEvaluationPlan;

public class CreateEvaluationPlanCommandHandler : IRequestHandler<CreateEvaluationPlanCommand, ErrorOr<EvaluationPlanResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateEvaluationPlanCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<EvaluationPlanResult>> Handle(CreateEvaluationPlanCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var subject = await _db.Subjects
            .Include(s => s.GradeLevel)
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (subject is null)
            return Error.NotFound(description: "Materia no encontrada.");

        var period = await _db.AcademicPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId && p.IsActive, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        // Validate total weight for this subject/period does not exceed 100
        var existingWeight = await _db.EvaluationPlans
            .Where(ep => ep.SubjectId == request.SubjectId && ep.AcademicPeriodId == request.AcademicPeriodId)
            .SumAsync(ep => ep.Weight, cancellationToken);

        if (existingWeight + request.Weight > 100)
            return Error.Validation(
                description: $"El peso total excede 100%. Peso disponible: {100 - existingWeight}%.");

        var plan = EvaluationPlan.Create(
            tenantId: tenantId,
            subjectId: request.SubjectId,
            academicPeriodId: request.AcademicPeriodId,
            name: request.Name,
            weight: request.Weight,
            description: request.Description,
            dueDate: request.DueDate);

        _db.EvaluationPlans.Add(plan);
        await _db.SaveChangesAsync(cancellationToken);

        return new EvaluationPlanResult(
            Id: plan.Id,
            SubjectId: subject.Id,
            SubjectName: subject.Name,
            AcademicPeriodId: period.Id,
            PeriodName: period.Name,
            Name: plan.Name,
            Weight: plan.Weight,
            Description: plan.Description,
            DueDate: plan.DueDate,
            IsPublished: plan.IsPublished,
            CreatedAt: plan.CreatedAt);
    }
}
