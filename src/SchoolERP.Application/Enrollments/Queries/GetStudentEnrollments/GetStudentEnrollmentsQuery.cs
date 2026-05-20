using ErrorOr;
using MediatR;
using SchoolERP.Application.Enrollments.Queries.GetSectionEnrollments;

namespace SchoolERP.Application.Enrollments.Queries.GetStudentEnrollments;

public record GetStudentEnrollmentsQuery(Guid StudentId) : IRequest<ErrorOr<List<EnrollmentResult>>>;
