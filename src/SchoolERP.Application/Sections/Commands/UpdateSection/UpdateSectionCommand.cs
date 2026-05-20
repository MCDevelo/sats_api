using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Commands.UpdateSection;

public record UpdateSectionCommand(
    Guid SectionId,
    string Name,
    Shift Shift,
    int Capacity,
    string? Classroom = null) : IRequest<ErrorOr<Success>>;
