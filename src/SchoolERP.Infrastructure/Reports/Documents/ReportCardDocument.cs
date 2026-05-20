using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Reports.Documents;

internal class ReportCardDocument : IDocument
{
    private readonly ReportCardData _d;

    public ReportCardDocument(ReportCardData data) => _d = data;

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
                        inner.Item().Text("Boletín de Calificaciones").FontSize(11).Bold();
                        inner.Item().Text($"Año Académico: {_d.AcademicYear}  |  Período: {_d.PeriodName}");
                    });
                    row.ConstantItem(110).AlignRight()
                        .Text($"Generado: {_d.GeneratedAt:dd/MM/yyyy}")
                        .FontSize(7).FontColor(Colors.Grey.Medium);
                });
                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text(t =>
                        {
                            t.Span("Estudiante: ").Bold();
                            t.Span(_d.StudentFullName);
                        });
                        inner.Item().Text(t =>
                        {
                            t.Span("Grado: ").Bold();
                            t.Span($"{_d.GradeLevel}  |  Sección: {_d.SectionName}");
                        });
                    });
                    row.RelativeItem().Column(inner =>
                    {
                        if (_d.StudentCode is not null)
                            inner.Item().Text(t => { t.Span("Código: ").Bold(); t.Span(_d.StudentCode); });
                        if (_d.Rank.HasValue)
                            inner.Item().Text(t => { t.Span("Puesto: ").Bold(); t.Span($"{_d.Rank}° de {_d.TotalStudents}"); });
                    });
                });
                col.Item().PaddingTop(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            });

            page.Content().PaddingTop(8).Column(col =>
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(50);
                        cols.ConstantColumn(55);
                        cols.ConstantColumn(60);
                        cols.ConstantColumn(36);
                        cols.ConstantColumn(50);
                    });

                    table.Header(header =>
                    {
                        var headers = new[] { "Materia", "Evaluación", "Peso %", "Nota", "Promedio", "Letra", "Estado" };
                        foreach (var h in headers)
                        {
                            var text = h;
                            header.Cell().Background("#2C5F8A").Padding(4).AlignCenter()
                                .Text(t => t.Span(text).FontColor(Colors.White).Bold().FontSize(8));
                        }
                    });

                    foreach (var subject in _d.Subjects)
                    {
                        var subjectBg = subject.IsPassing ? "#ffffff" : "#fff3f3";
                        var averageColor = subject.IsPassing ? "#2e7d32" : "#c62828";
                        var evalCount = subject.Evaluations.Count;

                        for (var i = 0; i < evalCount; i++)
                        {
                            var eval = subject.Evaluations[i];
                            var rowBg = i % 2 == 0 ? subjectBg : "#f5f5f5";

                            if (i == 0)
                            {
                                table.Cell().RowSpan((uint)evalCount)
                                    .Background(subjectBg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4).AlignMiddle()
                                    .Text(t => t.Span(subject.SubjectName).Bold().FontSize(8));
                            }

                            table.Cell().Background(rowBg).Padding(3).Text(eval.Name);
                            table.Cell().Background(rowBg).Padding(3).AlignCenter().Text($"{eval.Weight:F0}%");
                            table.Cell().Background(rowBg).Padding(3).AlignCenter().Text($"{eval.Score:F1}");

                            if (i == 0)
                            {
                                table.Cell().RowSpan((uint)evalCount)
                                    .Background(subjectBg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4).AlignCenter().AlignMiddle()
                                    .Text(t => t.Span($"{subject.WeightedAverage:F2}").FontColor(averageColor).Bold().FontSize(9));

                                table.Cell().RowSpan((uint)evalCount)
                                    .Background(subjectBg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4).AlignCenter().AlignMiddle()
                                    .Text(t => t.Span(subject.LetterGrade).Bold().FontSize(9));

                                table.Cell().RowSpan((uint)evalCount)
                                    .Background(subjectBg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4).AlignCenter().AlignMiddle()
                                    .Text(t => t.Span(subject.IsPassing ? "Aprobado" : "Reprobado")
                                        .FontColor(averageColor).Bold().FontSize(8));
                            }
                        }
                    }
                });

                col.Item().PaddingTop(12).Row(row =>
                {
                    row.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(inner =>
                    {
                        inner.Item().Text("Resumen").Bold().FontSize(10);
                        inner.Item().PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().Text(t =>
                            {
                                t.Span("Promedio general: ").Bold();
                                t.Span($"{_d.GeneralAverage:F2}");
                            });
                            r.RelativeItem().Text(t =>
                            {
                                t.Span("Estado: ").Bold();
                                t.Span(_d.IsPromoted ? "PROMOVIDO" : "NO PROMOVIDO")
                                 .FontColor(_d.IsPromoted ? "#2e7d32" : "#c62828").Bold();
                            });
                        });

                        if (_d.Attendance is not null)
                        {
                            inner.Item().PaddingTop(6).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            inner.Item().PaddingTop(4).Text("Asistencia").Bold();
                            inner.Item().Row(r =>
                            {
                                r.RelativeItem().Text(
                                    $"Presente: {_d.Attendance.Present}  Ausente: {_d.Attendance.Absent}" +
                                    $"  Tardanza: {_d.Attendance.Late}  Excusado: {_d.Attendance.Excused}");
                                r.AutoItem().Text(t =>
                                    t.Span($"Tasa: {_d.Attendance.AttendanceRate:F1}%").Bold());
                            });
                        }
                    });
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
