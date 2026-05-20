using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Students.Commands.EnrollStudent;

public record EnrollStudentCommand(
    Guid StudentId,
    Guid SectionId,
    Guid AcademicYearId,
    DateTime? EnrollmentDate = null) : IRequest<ErrorOr<EnrollmentResult>>;

public record EnrollmentResult(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentFullName,
    Guid SectionId,
    string SectionName,
    Guid AcademicYearId,
    string AcademicYearName,
    string Status,
    DateTime EnrollmentDate);
