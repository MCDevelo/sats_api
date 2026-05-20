using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Reports.Documents;

internal class EnrollmentReportDocument : IDocument
{
    private readonly EnrollmentReportData _d;

    public EnrollmentReportDocument(EnrollmentReportData data) => _d = data;

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
                        inner.Item().Text("Reporte de Matrícula").FontSize(11).Bold();
                        inner.Item().Text($"Año Académico: {_d.AcademicYearName}");
                    });
                    row.ConstantItem(130).AlignRight().Column(inner =>
                    {
                        inner.Item().Text($"Generado: {_d.GeneratedAt:dd/MM/yyyy}")
                            .FontSize(7).FontColor(Colors.Grey.Medium);
                        inner.Item().PaddingTop(2)
                            .Text(t => t.Span($"Total matriculados: {_d.TotalEnrolled}").Bold().FontSize(9));
                    });
                });
                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });

            page.Content().PaddingTop(8).Column(mainCol =>
            {
                foreach (var grade in _d.GradeLevels)
                {
                    mainCol.Item().PaddingTop(8).Column(gradeCol =>
                    {
                        gradeCol.Item().Background("#2C5F8A").Padding(5).Row(row =>
                        {
                            row.RelativeItem()
                                .Text(t => t.Span(grade.GradeName).FontColor(Colors.White).Bold().FontSize(10));
                            row.AutoItem()
                                .Text(t => t.Span(
                                    $"Total: {grade.TotalEnrolled}  |  Varones: {grade.TotalMale}  |  Hembras: {grade.TotalFemale}")
                                    .FontColor(Colors.White).FontSize(8));
                        });

                        gradeCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                foreach (var h in new[] { "Sección", "Matriculados", "Varones", "Hembras" })
                                {
                                    var text = h;
                                    header.Cell().Background("#5c8ab0").Padding(4).AlignCenter()
                                        .Text(t => t.Span(text).FontColor(Colors.White).Bold().FontSize(8));
                                }
                            });

                            var idx = 0;
                            foreach (var sec in grade.Sections)
                            {
                                var bg = idx++ % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                                table.Cell().Background(bg).Padding(3).Text(sec.SectionName);
                                table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{sec.Enrolled}");
                                table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{sec.Male}");
                                table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{sec.Female}");
                            }
                        });
                    });
                }

                var totalMale   = _d.GradeLevels.Sum(g => g.TotalMale);
                var totalFemale = _d.GradeLevels.Sum(g => g.TotalFemale);

                mainCol.Item().PaddingTop(14)
                    .Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(t =>
                    {
                        t.Span("TOTAL GENERAL: ").Bold();
                        t.Span($"{_d.TotalEnrolled} estudiantes  |  Varones: {totalMale}  |  Hembras: {totalFemale}");
                    });
            });

            page.Footer().AlignCenter().Text(t =>
            {
                t.Span("Página ").FontSize(7).FontColor(Colors.Grey.Medium);
                t.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Medium);
                t.Span(" de ").FontSize(7).FontColor(Colors.Grey.Medium);
                t.TotalPages().FontSize(7).FontColor(Colors.Grey.Medium);
            });
        });
    }
}
