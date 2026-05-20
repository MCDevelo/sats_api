using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Commands.CreateSection;

public record CreateSectionCommand(
    Guid SchoolId,
    Guid GradeLevelId,
    Guid AcademicYearId,
    string Name,
    Shift Shift,
    int Capacity = 35,
    string? Classroom = null) : IRequest<ErrorOr<Guid>>;
