using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Enrollments.Queries.GetSectionEnrollments;

public record GetSectionEnrollmentsQuery(
    Guid SectionId,
    EnrollmentStatus? Status = null,
    int Page = 1,
    int PageSize = 50) : IRequest<ErrorOr<PagedResult<EnrollmentResult>>>;

public record EnrollmentResult(
    Guid Id,
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    string? StudentNse,
    string StudentGender,
    string? StudentPhotoUrl,
    Guid SectionId,
    string SectionName,
    Guid AcademicYearId,
    string AcademicYearName,
    EnrollmentStatus Status,
    DateTime EnrollmentDate,
    DateTime? WithdrawalDate,
    string? WithdrawalReason,
    string? Notes,
    DateTime CreatedAt);
