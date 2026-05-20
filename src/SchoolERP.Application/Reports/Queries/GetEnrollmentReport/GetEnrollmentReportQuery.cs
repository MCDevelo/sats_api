using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Reports.Queries.GetEnrollmentReport;

public record GetEnrollmentReportQuery(
    Guid SchoolId,
    Guid AcademicYearId) : IRequest<ErrorOr<ReportResult>>;
