using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Reports.Queries.GetAttendanceReport;

public record GetAttendanceReportQuery(
    Guid SectionId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<ReportResult>>;
