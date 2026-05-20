using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Reports.Documents;

internal class AttendanceReportDocument : IDocument
{
    private readonly AttendanceReportData _d;

    public AttendanceReportDocument(AttendanceReportData data) => _d = data;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text(_d.SchoolName).FontSize(13).Bold();
                        inner.Item().Text("Reporte de Asistencia").FontSize(11).Bold();
                        inner.Item().Text($"{_d.GradeLevel} — Sección {_d.SectionName}");
                        inner.Item().Text($"Período: {_d.PeriodName} ({_d.StartDate:dd/MM/yyyy} – {_d.EndDate:dd/MM/yyyy})");
                    });
                    row.ConstantItem(130).AlignRight()
                        .Text($"Generado: {_d.GeneratedAt:dd/MM/yyyy HH:mm}")
                        .FontSize(7).FontColor(Colors.Grey.Medium);
                });
                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });

            page.Content().PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(22);
                    cols.RelativeColumn(4);
                    cols.RelativeColumn(1.5f);
                    cols.ConstantColumn(36);
                    cols.ConstantColumn(36);
                    cols.ConstantColumn(36);
                    cols.ConstantColumn(36);
                    cols.ConstantColumn(40);
                    cols.ConstantColumn(48);
                });

                table.Header(header =>
                {
                    var headers = new[] { "#", "Estudiante", "Código", "Pres.", "Ause.", "Tard.", "Excus.", "Total", "% Asist." };
                    foreach (var h in headers)
                    {
                        var text = h; // capture for closure
                        header.Cell().Background("#2C5F8A").Padding(4).AlignCenter()
                            .Text(t => t.Span(text).FontColor(Colors.White).Bold().FontSize(8));
                    }
                });

                foreach (var row in _d.Rows)
                {
                    var bg = row.Index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                    var rateColor = row.AttendanceRate >= 90 ? "#2e7d32"
                                 : row.AttendanceRate >= 75 ? "#e65100"
                                 : "#c62828";

                    table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{row.Index}");
                    table.Cell().Background(bg).Padding(3).Text(row.FullName);
                    table.Cell().Background(bg).Padding(3).AlignCenter().Text(row.StudentCode ?? "");
                    table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{row.Present}");
                    table.Cell().Background(bg).Padding(3).AlignCenter()
                        .Text(t => t.Span($"{row.Absent}").FontColor(Colors.Red.Medium));
                    table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{row.Late}");
                    table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{row.Excused}");
                    table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{row.TotalDays}");
                    table.Cell().Background(bg).Padding(3).AlignCenter()
                        .Text(t => t.Span($"{row.AttendanceRate:F1}%").FontColor(rateColor).Bold());
                }
            });

            page.Footer().Row(row =>
            {
                row.RelativeItem().AlignLeft()
                    .Text($"Total estudiantes: {_d.Rows.Count}").FontSize(7).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignCenter().Text(t =>
                {
                    t.Span("Página ").FontSize(7).FontColor(Colors.Grey.Medium);
                    t.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Medium);
                    t.Span(" de ").FontSize(7).FontColor(Colors.Grey.Medium);
                    t.TotalPages().FontSize(7).FontColor(Colors.Grey.Medium);
                });
                row.RelativeItem().AlignRight()
                    .Text("Sistema ERP Escolar").FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }
}
