using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Sections.Commands.DeactivateSection;

public record DeactivateSectionCommand(Guid SectionId) : IRequest<ErrorOr<Success>>;
