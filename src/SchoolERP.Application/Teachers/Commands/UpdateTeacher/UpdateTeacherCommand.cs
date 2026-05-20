using ErrorOr;
using MediatR;
using SchoolERP.Application.Teachers.Commands.CreateTeacher;

namespace SchoolERP.Application.Teachers.Commands.UpdateTeacher;

public record UpdateTeacherCommand(
    Guid TeacherId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Address,
    string? Specialization,
    string? Qualifications,
    string? AcademicTitle,
    int WorkingHoursPerWeek,
    DateTime? ContractEndDate) : IRequest<ErrorOr<TeacherResult>>;
