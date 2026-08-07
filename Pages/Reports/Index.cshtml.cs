using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Reports
{
    [Authorize(Roles = "SuperUser,Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalBookings { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public IList<Booking> Bookings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            TotalBookings = await _context.Bookings.CountAsync();
            ApprovedCount = await _context.Bookings.CountAsync(b => b.Status == "Approved");
            PendingCount = await _context.Bookings.CountAsync(b => b.Status == "Pending");

            Bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Department)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        // Handler untuk Export ke Excel
        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Department)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Laporan Peminjaman");
                
                worksheet.Cell(1, 1).Value = "No";
                worksheet.Cell(1, 2).Value = "Pemohon";
                worksheet.Cell(1, 3).Value = "Departemen";
                worksheet.Cell(1, 4).Value = "Ruangan";
                worksheet.Cell(1, 5).Value = "Tanggal";
                worksheet.Cell(1, 6).Value = "Waktu Mulai";
                worksheet.Cell(1, 7).Value = "Waktu Selesai";
                worksheet.Cell(1, 8).Value = "Status";

                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;

                int row = 2;
                int no = 1;
                foreach (var item in bookings)
                {
                    worksheet.Cell(row, 1).Value = no++;
                    worksheet.Cell(row, 2).Value = item.User?.Email ?? "-";
                    worksheet.Cell(row, 3).Value = item.Department?.DepartmentName ?? "-";
                    worksheet.Cell(row, 4).Value = item.Room?.RoomName ?? "-";
                    worksheet.Cell(row, 5).Value = item.StartTime.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 6).Value = item.StartTime.ToString("HH:mm");
                    worksheet.Cell(row, 7).Value = item.EndTime.ToString("HH:mm");
                    worksheet.Cell(row, 8).Value = item.Status;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"Laporan_Peminjaman_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Department)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Header PDF
                    page.Header().ShowOnce().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Laporan Peminjaman Ruangan").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Dicetak pada: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    // Content / Isi PDF
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);   // No
                            columns.RelativeColumn(3);    // Pemohon
                            columns.RelativeColumn(2);    // Departemen
                            columns.RelativeColumn(2);    // Ruangan
                            columns.RelativeColumn(2);    // Tanggal
                            columns.RelativeColumn(2);    // Waktu
                            columns.RelativeColumn(1.5f); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("No").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Pemohon").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Departemen").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Ruangan").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Tanggal").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Waktu").Bold();
                            header.Cell().Element(HeaderCellStyle).Text("Status").Bold();

                            static IContainer HeaderCellStyle(IContainer container) => 
                                container.Border(1).BorderColor(Colors.Blue.Darken4)
                                        .Background(Colors.Blue.Darken2)
                                        .DefaultTextStyle(x => x.FontColor(Colors.White))
                                        .Padding(5);
                        });

                        int no = 1;
                        foreach (var item in bookings)
                        {
                            table.Cell().Element(CellBodyStyle).Text(no++.ToString());
                            table.Cell().Element(CellBodyStyle).Text(item.User?.Email ?? "-");
                            table.Cell().Element(CellBodyStyle).Text(item.Department?.DepartmentName ?? "-");
                            table.Cell().Element(CellBodyStyle).Text(item.Room?.RoomName ?? "-");
                            table.Cell().Element(CellBodyStyle).Text(item.StartTime.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellBodyStyle).Text($"{item.StartTime:HH:mm} - {item.EndTime:HH:mm}");
                            table.Cell().Element(CellBodyStyle).Text(item.Status);

                            static IContainer CellBodyStyle(IContainer container) => 
                                container.Border(1).BorderColor(Colors.Grey.Lighten1)
                                        .Padding(5);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Halaman ");
                        x.CurrentPageNumber();
                        x.Span(" dari ");
                        x.TotalPages();
                    });
                });
            });

            byte[] pdfBytes = pdfDocument.GeneratePdf();
            string fileName = $"Laporan_Peminjaman_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            var stream = new MemoryStream(pdfBytes);
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}