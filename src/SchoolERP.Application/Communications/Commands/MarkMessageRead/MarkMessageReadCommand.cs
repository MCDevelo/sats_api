using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Communications.Commands.MarkMessageRead;

public record MarkMessageReadCommand(Guid MessageId) : IRequest<ErrorOr<Success>>;
