namespace SchoolERP.Application.Common.Models;

public record ReportResult(
    byte[] Content,
    string FileName,
    string ContentType = "application/pdf");
