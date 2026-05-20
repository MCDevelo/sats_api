using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Infrastructure.Reports.Documents;

namespace SchoolERP.Infrastructure.Reports;

public class QuestPdfReportGeneratorService : IReportGeneratorService
{
    static QuestPdfReportGeneratorService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateAttendanceReport(AttendanceReportData data)
        => new AttendanceReportDocument(data).GeneratePdf();

    public byte[] GenerateReportCard(ReportCardData data)
        => new ReportCardDocument(data).GeneratePdf();

    public byte[] GenerateFinancialReport(FinancialReportData data)
        => new FinancialReportDocument(data).GeneratePdf();

    public byte[] GenerateEnrollmentReport(EnrollmentReportData data)
        => new EnrollmentReportDocument(data).GeneratePdf();
}
