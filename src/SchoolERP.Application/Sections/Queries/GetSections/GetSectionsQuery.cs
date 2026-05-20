using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Queries.GetSections;

public record GetSectionsQuery(
    Guid? SchoolId = null,
    Guid? GradeLevelId = null,
    Guid? AcademicYearId = null,
    Shift? Shift = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 50) : IRequest<ErrorOr<PagedResult<SectionResult>>>;

public record SectionResult(
    Guid Id,
    Guid SchoolId,
    Guid GradeLevelId,
    string GradeLevelName,
    Guid AcademicYearId,
    string AcademicYearName,
    string Name,
    string Shift,
    int Capacity,
    int EnrolledCount,
    Guid? HomeTeacherId,
    string? HomeTeacherName,
    string? Classroom,
    bool IsActive,
    DateTime CreatedAt);
