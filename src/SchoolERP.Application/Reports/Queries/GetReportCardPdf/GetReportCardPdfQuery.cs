using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Reports.Queries.GetReportCardPdf;

public record GetReportCardPdfQuery(
    Guid StudentId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<ReportResult>>;
