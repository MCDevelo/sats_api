using ErrorOr;
using MediatR;
using SchoolERP.Application.Users.Queries.GetUsers;

namespace SchoolERP.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<UserResult>>;
