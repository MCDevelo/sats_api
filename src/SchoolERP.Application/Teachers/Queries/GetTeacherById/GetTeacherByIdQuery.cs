using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherById;

public record GetTeacherByIdQuery(Guid TeacherId) : IRequest<ErrorOr<TeacherDetailResult>>;

public record TeacherDetailResult(
    Guid Id,
    Guid TenantId,
    Guid SchoolId,
    string SchoolName,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    string? NationalId,
    string? MinerdCode,
    string? TeacherCode,
    string? AcademicTitle,
    string? Specialization,
    string? Qualifications,
    string ContractType,
    DateTime HireDate,
    DateTime? ContractEndDate,
    int WorkingHoursPerWeek,
    string? Gender,
    DateTime? DateOfBirth,
    string? Address,
    string? PhotoUrl,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<TeacherAssignmentSummary> CurrentAssignments);

public record TeacherAssignmentSummary(
    Guid AssignmentId,
    string AcademicYear,
    string GradeLevel,
    string Section,
    string Subject,
    string Shift);
