using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models;
using ITServiceAssetManagement.Models.Entities;
using ITServiceAssetManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.Controllers
{
    [Authorize(Roles = "IT_SERVICE,IT_SUPPORT,PURCHASING")]
    public class ReplacementRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;
        private readonly INotificationService _notification;

        public ReplacementRequestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuditLogService auditLog,
            INotificationService notification)
        {
            _context = context;
            _userManager = userManager;
            _auditLog = auditLog;
            _notification = notification;
        }

        // GET: ReplacementRequest
        public async Task<IActionResult> Index(string? status, int pageIndex = 1)
        {
            var query = _context.ReplacementRequests
                .Include(r => r.Asset)
                .Include(r => r.Ticket)
                .Include(r => r.RequestedBy)
                .Include(r => r.ProcessedBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            query = query.OrderByDescending(r => r.RequestedAt);

            var result = await PaginatedList<ReplacementRequest>.CreateAsync(query, pageIndex, 10);

            ViewBag.Status = status;

            return View(result);
        }

        // GET: ReplacementRequest/Create?ticketId=5&assetId=3
        [Authorize(Roles = "IT_SERVICE,IT_SUPPORT")]
        public IActionResult Create(int? ticketId, int? assetId)
        {
            ViewBag.Assets = _context.Assets.OrderBy(a => a.AssetName).ToList();
            return View(new ReplacementRequestCreateViewModel
            {
                TicketId = ticketId,
                AssetId = assetId ?? 0
            });
        }

        // POST: ReplacementRequest/Create
        [HttpPost]
        [Authorize(Roles = "IT_SERVICE,IT_SUPPORT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReplacementRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Assets = _context.Assets.OrderBy(a => a.AssetName).ToList();
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            var request = new ReplacementRequest
            {
                TicketId = model.TicketId,
                AssetId = model.AssetId,
                Reason = model.Reason,
                RequestedById = userId,
                Status = "PENDING",
                RequestedAt = DateTime.UtcNow
            };

            _context.ReplacementRequests.Add(request);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("ReplacementRequest", request.Id, "CREATE", $"Pengajuan penggantian dibuat: {model.Reason}", userId);

            // Notifikasi ke semua PURCHASING
            var asset = await _context.Assets.FindAsync(model.AssetId);
            await _notification.NotifyRoleAsync(
                "PURCHASING",
                "Pengajuan Penggantian Baru",
                $"Ada pengajuan penggantian asset '{asset?.AssetName}' yang perlu diproses.",
                $"/ReplacementRequest/Process/{request.Id}");

            TempData["Success"] = "Pengajuan penggantian asset berhasil dibuat.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ReplacementRequest/Process/5 (PURCHASING)
        [Authorize(Roles = "PURCHASING")]
        public async Task<IActionResult> Process(int id)
        {
            var request = await _context.ReplacementRequests
                .Include(r => r.Asset)
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            return View(new ReplacementProcessViewModel { Id = id });
        }

        // POST: ReplacementRequest/Process
        [HttpPost]
        [Authorize(Roles = "PURCHASING")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(ReplacementProcessViewModel model)
        {
            var request = await _context.ReplacementRequests
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (request == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var oldStatus = request.Status;

            request.Status = model.Status;
            request.ProcessedById = userId;
            request.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("ReplacementRequest", request.Id, "STATUS_CHANGE", $"Status diubah dari {oldStatus} ke {model.Status}.", userId);

            // Notifikasi ke yang mengajukan
            await _notification.NotifyAsync(
                request.RequestedById,
                "Pengajuan Penggantian Diproses",
                $"Pengajuan penggantian '{request.Asset?.AssetName}' telah di-{model.Status}.",
                $"/ReplacementRequest/Index");

            TempData["Success"] = "Pengajuan berhasil diproses.";
            return RedirectToAction(nameof(Index));
        }
    }
}
