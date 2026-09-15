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
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;
        private readonly INotificationService _notification;
        private readonly ISlaService _sla;
        private readonly IFileStorageService _fileStorage;

        public TicketController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuditLogService auditLog,
            INotificationService notification,
            ISlaService sla,
            IFileStorageService fileStorage)
        {
            _context = context;
            _userManager = userManager;
            _auditLog = auditLog;
            _notification = notification;
            _sla = sla;
            _fileStorage = fileStorage;
        }

        // GET: Ticket
        public async Task<IActionResult> Index(string? status, string? priority, string? search, bool? overdueOnly, int pageIndex = 1)
        {
            var userId = _userManager.GetUserId(User);
            var isEmployee = User.IsInRole("EMPLOYEE")
                && !User.IsInRole("IT_SERVICE")
                && !User.IsInRole("IT_SUPPORT")
                && !User.IsInRole("PURCHASING");

            var query = _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Asset)
                .Include(t => t.TicketAssignments)
                    .ThenInclude(ta => ta.AssignedTo)
                .AsQueryable();

            if (isEmployee)
            {
                query = query.Where(t => t.RequesterId == userId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(t => t.Priority == priority);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.Contains(search) || (t.Description != null && t.Description.Contains(search)));
            }

            if (overdueOnly == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(t => t.DueDate != null && now > t.DueDate
                    && t.Status != "RESOLVED" && t.Status != "CLOSED");
            }

            query = query.OrderByDescending(t => t.CreatedAt);

            var result = await PaginatedList<Ticket>.CreateAsync(query, pageIndex, 10);

            ViewBag.Status = status;
            ViewBag.Priority = priority;
            ViewBag.Search = search;
            ViewBag.OverdueOnly = overdueOnly;

            return View(result);
        }

        // GET: Ticket/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Requester)
                .Include(t => t.Asset)
                .Include(t => t.TicketAssignments).ThenInclude(ta => ta.AssignedTo)
                .Include(t => t.Troubleshootings).ThenInclude(tr => tr.PerformedBy)
                .Include(t => t.Troubleshootings).ThenInclude(tr => tr.Attachments)
                .Include(t => t.ReplacementRequests)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isOwnerEmployee = User.IsInRole("EMPLOYEE") && ticket.RequesterId == userId;
            var isStaff = User.IsInRole("IT_SERVICE") || User.IsInRole("IT_SUPPORT") || User.IsInRole("PURCHASING");

            if (!isOwnerEmployee && !isStaff) return Forbid();

            return View(ticket);
        }

        // GET: Ticket/Create
        public IActionResult Create()
        {
            ViewBag.Assets = _context.Assets.OrderBy(a => a.AssetName).ToList();
            return View(new TicketCreateViewModel());
        }

        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Assets = _context.Assets.OrderBy(a => a.AssetName).ToList();
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var createdAt = DateTime.UtcNow;

            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                AssetId = model.AssetId,
                RequesterId = userId,
                Status = "OPEN",
                CreatedAt = createdAt,
                DueDate = _sla.CalculateDueDate(model.Priority, createdAt)
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Upload attachment kalau ada
            if (model.AttachmentFile != null)
            {
                try
                {
                    var (fileName, filePath) = await _fileStorage.SaveFileAsync(model.AttachmentFile, "tickets");
                    _context.Attachments.Add(new Attachment
                    {
                        TicketId = ticket.Id,
                        FileName = fileName,
                        FilePath = filePath,
                        UploadedById = userId,
                        UploadedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }
                catch (InvalidOperationException ex)
                {
                    TempData["UploadError"] = ex.Message;
                }
            }

            await _auditLog.LogAsync("Ticket", ticket.Id, "CREATE", $"Tiket '{ticket.Title}' dibuat. SLA due: {ticket.DueDate:dd-MM-yyyy HH:mm}.", userId);

            await _notification.NotifyRoleAsync(
                "IT_SERVICE",
                "Tiket Baru",
                $"Tiket baru '{ticket.Title}' (Prioritas: {ticket.Priority}) menunggu untuk di-assign.",
                $"/Ticket/Details/{ticket.Id}");

            TempData["Success"] = "Tiket berhasil dibuat.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Ticket/Assign/5 (IT_SERVICE)
        [Authorize(Roles = "IT_SERVICE")]
        public async Task<IActionResult> Assign(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var itSupportUsers = await _userManager.GetUsersInRoleAsync("IT_SUPPORT");
            ViewBag.ItSupportUsers = itSupportUsers;
            ViewBag.Ticket = ticket;

            return View(new TicketAssignViewModel { TicketId = id });
        }

        // POST: Ticket/Assign
        [HttpPost]
        [Authorize(Roles = "IT_SERVICE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(TicketAssignViewModel model)
        {
            var ticket = await _context.Tickets.FindAsync(model.TicketId);
            if (ticket == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ItSupportUsers = await _userManager.GetUsersInRoleAsync("IT_SUPPORT");
                ViewBag.Ticket = ticket;
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            var assignment = new TicketAssignment
            {
                TicketId = model.TicketId,
                AssignedToId = model.AssignedToId,
                Status = "ASSIGNED",
                AssignedAt = DateTime.UtcNow
            };

            ticket.Status = "IN_PROGRESS";

            _context.TicketAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            var assignedUser = await _userManager.FindByIdAsync(model.AssignedToId);
            await _auditLog.LogAsync("Ticket", ticket.Id, "ASSIGN", $"Tiket ditugaskan ke {assignedUser?.FullName}.", userId);

            await _notification.NotifyAsync(
                model.AssignedToId,
                "Tiket Ditugaskan ke Anda",
                $"Tiket '{ticket.Title}' telah ditugaskan kepada Anda. Batas waktu: {ticket.DueDate:dd-MM-yyyy HH:mm}.",
                $"/Ticket/Details/{ticket.Id}");

            await _notification.NotifyAsync(
                ticket.RequesterId,
                "Tiket Sedang Diproses",
                $"Tiket Anda '{ticket.Title}' sedang ditangani oleh tim IT Support.",
                $"/Ticket/Details/{ticket.Id}");

            TempData["Success"] = "Tiket berhasil ditugaskan.";
            return RedirectToAction(nameof(Details), new { id = model.TicketId });
        }

        // GET: Ticket/AddTroubleshooting/5 (IT_SUPPORT)
        [Authorize(Roles = "IT_SUPPORT")]
        public async Task<IActionResult> AddTroubleshooting(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewBag.Ticket = ticket;
            return View(new TroubleshootingCreateViewModel { TicketId = id });
        }

        // POST: Ticket/AddTroubleshooting
        [HttpPost]
        [Authorize(Roles = "IT_SUPPORT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTroubleshooting(TroubleshootingCreateViewModel model)
        {
            var ticket = await _context.Tickets.FindAsync(model.TicketId);
            if (ticket == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Ticket = ticket;
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            var troubleshooting = new Troubleshooting
            {
                TicketId = model.TicketId,
                ActionTaken = model.ActionTaken,
                Notes = model.Notes,
                Result = model.Result,
                PerformedById = userId,
                PerformedAt = DateTime.UtcNow
            };

            _context.Troubleshootings.Add(troubleshooting);

            if (model.Result == "SUCCESS")
            {
                ticket.Status = "RESOLVED";
            }

            await _context.SaveChangesAsync();

            // Upload attachment bukti perbaikan kalau ada
            if (model.AttachmentFile != null)
            {
                try
                {
                    var (fileName, filePath) = await _fileStorage.SaveFileAsync(model.AttachmentFile, "troubleshooting");
                    _context.Attachments.Add(new Attachment
                    {
                        TroubleshootingId = troubleshooting.Id,
                        FileName = fileName,
                        FilePath = filePath,
                        UploadedById = userId,
                        UploadedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }
                catch (InvalidOperationException ex)
                {
                    TempData["UploadError"] = ex.Message;
                }
            }

            await _auditLog.LogAsync("Ticket", ticket.Id, "TROUBLESHOOTING", $"Troubleshooting dicatat: {model.ActionTaken} (Result: {model.Result})", userId);

            if (model.Result == "SUCCESS")
            {
                await _notification.NotifyAsync(
                    ticket.RequesterId,
                    "Tiket Selesai",
                    $"Tiket Anda '{ticket.Title}' telah selesai ditangani.",
                    $"/Ticket/Details/{ticket.Id}");
            }

            TempData["Success"] = "Troubleshooting berhasil dicatat.";
            return RedirectToAction(nameof(Details), new { id = model.TicketId });
        }

        // POST: Ticket/Close/5
        [Authorize(Roles = "IT_SERVICE,IT_SUPPORT")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var oldStatus = ticket.Status;

            ticket.Status = "CLOSED";
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("Ticket", ticket.Id, "STATUS_CHANGE", $"Status diubah dari {oldStatus} ke CLOSED.", userId);

            await _notification.NotifyAsync(
                ticket.RequesterId,
                "Tiket Ditutup",
                $"Tiket Anda '{ticket.Title}' telah ditutup.",
                $"/Ticket/Details/{ticket.Id}");

            TempData["Success"] = "Tiket berhasil ditutup.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
