using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Reports.Queries.GetFinancialReport;

public record GetFinancialReportQuery(
    Guid SchoolId,
    DateOnly DateFrom,
    DateOnly DateTo) : IRequest<ErrorOr<ReportResult>>;
