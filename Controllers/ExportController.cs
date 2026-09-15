using ClosedXML.Excel;
using ITServiceAssetManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.Controllers
{
    [Authorize(Roles = "IT_SERVICE,IT_SUPPORT,PURCHASING")]
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "IT_SERVICE,IT_SUPPORT")]
        public async Task<IActionResult> Assets()
        {
            var assets = await _context.Assets
                .OrderBy(a => a.AssetName)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Assets");

            ws.Cell(1, 1).Value = "Nama Asset";
            ws.Cell(1, 2).Value = "Kode";
            ws.Cell(1, 3).Value = "Kategori";
            ws.Cell(1, 4).Value = "Lokasi";
            ws.Cell(1, 5).Value = "Status";
            ws.Cell(1, 6).Value = "Tanggal Beli";
            ws.Range(1, 1, 1, 6).Style.Font.Bold = true;

            var row = 2;
            foreach (var a in assets)
            {
                ws.Cell(row, 1).Value = a.AssetName;
                ws.Cell(row, 2).Value = a.AssetCode;
                ws.Cell(row, 3).Value = a.Category;
                ws.Cell(row, 4).Value = a.Location;
                ws.Cell(row, 5).Value = a.Status;
                ws.Cell(row, 6).Value = a.PurchaseDate?.ToString("dd-MM-yyyy") ?? "";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Assets_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> Tickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Asset)
                .Include(t => t.TicketAssignments).ThenInclude(ta => ta.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tickets");

            ws.Cell(1, 1).Value = "Judul";
            ws.Cell(1, 2).Value = "Requester";
            ws.Cell(1, 3).Value = "Asset";
            ws.Cell(1, 4).Value = "Prioritas";
            ws.Cell(1, 5).Value = "Status";
            ws.Cell(1, 6).Value = "Ditugaskan ke";
            ws.Cell(1, 7).Value = "Tanggal Dibuat";
            ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

            var row = 2;
            foreach (var t in tickets)
            {
                var lastAssignment = t.TicketAssignments.OrderByDescending(a => a.AssignedAt).FirstOrDefault();

                ws.Cell(row, 1).Value = t.Title;
                ws.Cell(row, 2).Value = t.Requester?.FullName;
                ws.Cell(row, 3).Value = t.Asset?.AssetName;
                ws.Cell(row, 4).Value = t.Priority;
                ws.Cell(row, 5).Value = t.Status;
                ws.Cell(row, 6).Value = lastAssignment?.AssignedTo?.FullName;
                ws.Cell(row, 7).Value = t.CreatedAt.ToString("dd-MM-yyyy HH:mm");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Tickets_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> ReplacementRequests()
        {
            var requests = await _context.ReplacementRequests
                .Include(r => r.Asset)
                .Include(r => r.RequestedBy)
                .Include(r => r.ProcessedBy)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("ReplacementRequests");

            ws.Cell(1, 1).Value = "Asset";
            ws.Cell(1, 2).Value = "Alasan";
            ws.Cell(1, 3).Value = "Diajukan oleh";
            ws.Cell(1, 4).Value = "Status";
            ws.Cell(1, 5).Value = "Diproses oleh";
            ws.Cell(1, 6).Value = "Tanggal Diajukan";
            ws.Range(1, 1, 1, 6).Style.Font.Bold = true;

            var row = 2;
            foreach (var r in requests)
            {
                ws.Cell(row, 1).Value = r.Asset?.AssetName;
                ws.Cell(row, 2).Value = r.Reason;
                ws.Cell(row, 3).Value = r.RequestedBy?.FullName;
                ws.Cell(row, 4).Value = r.Status;
                ws.Cell(row, 5).Value = r.ProcessedBy?.FullName;
                ws.Cell(row, 6).Value = r.RequestedAt.ToString("dd-MM-yyyy HH:mm");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ReplacementRequests_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
