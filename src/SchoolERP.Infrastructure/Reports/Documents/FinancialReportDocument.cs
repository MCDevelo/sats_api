using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Reports.Documents;

internal class FinancialReportDocument : IDocument
{
    private readonly FinancialReportData _d;

    public FinancialReportDocument(FinancialReportData data) => _d = data;

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
                        inner.Item().Text("Reporte Financiero").FontSize(11).Bold();
                        inner.Item().Text($"Período: {_d.DateFrom:MMM yyyy} – {_d.DateTo:MMM yyyy}");
                    });
                    row.ConstantItem(110).AlignRight()
                        .Text($"Generado: {_d.GeneratedAt:dd/MM/yyyy}")
                        .FontSize(7).FontColor(Colors.Grey.Medium);
                });
                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });

            page.Content().PaddingTop(8).Column(col =>
            {
                // KPI summary boxes
                col.Item().Row(row =>
                {
                    KpiBox(row.RelativeItem(), "Total Facturado", _d.TotalBilled,    "#1565c0");
                    KpiBox(row.RelativeItem(), "Cobrado",         _d.TotalCollected, "#2e7d32");
                    KpiBox(row.RelativeItem(), "Pendiente",       _d.TotalPending,   "#e65100");
                    KpiBox(row.RelativeItem(), "Vencido",         _d.TotalOverdue,   "#c62828");
                });

                col.Item().PaddingTop(6).AlignCenter()
                    .Text(t => t.Span($"Tasa de cobro: {_d.CollectionRate:F1}%").Bold().FontSize(10));

                // Monthly breakdown
                col.Item().PaddingTop(14)
                    .Text(t => t.Span("Desglose mensual").Bold().FontSize(10));

                col.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Mes", "Facturado (RD$)", "Cobrado (RD$)", "Pendiente (RD$)" })
                        {
                            var text = h;
                            header.Cell().Background("#2C5F8A").Padding(4).AlignCenter()
                                .Text(t => t.Span(text).FontColor(Colors.White).Bold().FontSize(8));
                        }
                    });

                    var idx = 0;
                    foreach (var m in _d.Monthly)
                    {
                        var bg = idx++ % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                        table.Cell().Background(bg).Padding(3).Text(m.Month);
                        table.Cell().Background(bg).Padding(3).AlignRight()
                            .Text(t => t.Span($"{m.Billed:N2}").FontSize(8));
                        table.Cell().Background(bg).Padding(3).AlignRight()
                            .Text(t => t.Span($"{m.Collected:N2}").FontColor("#2e7d32").FontSize(8));
                        table.Cell().Background(bg).Padding(3).AlignRight()
                            .Text(t => t.Span($"{m.Pending:N2}").FontColor("#e65100").FontSize(8));
                    }

                    // Totals row
                    table.Cell().Background("#e3f2fd").Padding(3)
                        .Text(t => t.Span("TOTAL").Bold().FontSize(8));
                    table.Cell().Background("#e3f2fd").Padding(3).AlignRight()
                        .Text(t => t.Span($"{_d.TotalBilled:N2}").Bold().FontSize(8));
                    table.Cell().Background("#e3f2fd").Padding(3).AlignRight()
                        .Text(t => t.Span($"{_d.TotalCollected:N2}").Bold().FontColor("#2e7d32").FontSize(8));
                    table.Cell().Background("#e3f2fd").Padding(3).AlignRight()
                        .Text(t => t.Span($"{_d.TotalPending:N2}").Bold().FontColor("#e65100").FontSize(8));
                });

                if (_d.PaymentMethods.Count > 0)
                {
                    col.Item().PaddingTop(14)
                        .Text(t => t.Span("Métodos de pago").Bold().FontSize(10));

                    col.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            foreach (var h in new[] { "Método", "Monto (RD$)", "%" })
                            {
                                var text = h;
                                header.Cell().Background("#2C5F8A").Padding(4).AlignCenter()
                                    .Text(t => t.Span(text).FontColor(Colors.White).Bold().FontSize(8));
                            }
                        });

                        var idx = 0;
                        foreach (var pm in _d.PaymentMethods)
                        {
                            var bg = idx++ % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                            table.Cell().Background(bg).Padding(3).Text(pm.Method);
                            table.Cell().Background(bg).Padding(3).AlignRight().Text($"{pm.Amount:N2}");
                            table.Cell().Background(bg).Padding(3).AlignCenter().Text($"{pm.Percentage:F1}%");
                        }
                    });
                }
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

    private static void KpiBox(IContainer container, string label, decimal value, string color)
    {
        container.Padding(4).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.White).Padding(8).Column(col =>
            {
                col.Item().AlignCenter().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(4).AlignCenter()
                    .Text(t => t.Span($"RD$ {value:N2}").FontSize(10).Bold().FontColor(color));
            });
    }
}
