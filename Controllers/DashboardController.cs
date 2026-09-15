using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models;
using ITServiceAssetManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var isEmployeeOnly = User.IsInRole("EMPLOYEE")
                && !User.IsInRole("IT_SERVICE")
                && !User.IsInRole("IT_SUPPORT")
                && !User.IsInRole("PURCHASING");

            var ticketQuery = _context.Tickets.AsQueryable();
            if (isEmployeeOnly)
            {
                ticketQuery = ticketQuery.Where(t => t.RequesterId == userId);
            }

            var model = new DashboardViewModel
            {
                TotalTickets = await ticketQuery.CountAsync(),
                OpenTickets = await ticketQuery.CountAsync(t => t.Status == "OPEN"),
                InProgressTickets = await ticketQuery.CountAsync(t => t.Status == "IN_PROGRESS"),
                ResolvedTickets = await ticketQuery.CountAsync(t => t.Status == "RESOLVED"),
                ClosedTickets = await ticketQuery.CountAsync(t => t.Status == "CLOSED"),
            };

            var now = DateTime.UtcNow;
            model.OverdueTickets = await ticketQuery.CountAsync(t =>
                t.DueDate != null && now > t.DueDate && t.Status != "RESOLVED" && t.Status != "CLOSED");

            if (User.IsInRole("IT_SERVICE"))
            {
                model.UnassignedTickets = await _context.Tickets
                    .Where(t => t.Status == "OPEN" && !t.TicketAssignments.Any())
                    .CountAsync();
            }

            if (User.IsInRole("IT_SUPPORT"))
            {
                model.MyAssignedTickets = await _context.TicketAssignments
                    .Where(a => a.AssignedToId == userId)
                    .Select(a => a.TicketId)
                    .Distinct()
                    .CountAsync();

                model.MyOpenTickets = await _context.TicketAssignments
                    .Where(a => a.AssignedToId == userId && a.Ticket!.Status == "IN_PROGRESS")
                    .Select(a => a.TicketId)
                    .Distinct()
                    .CountAsync();
            }

            if (User.IsInRole("IT_SERVICE") || User.IsInRole("IT_SUPPORT"))
            {
                model.TotalAssets = await _context.Assets.CountAsync();
                model.ActiveAssets = await _context.Assets.CountAsync(a => a.Status == "ACTIVE");
                model.InRepairAssets = await _context.Assets.CountAsync(a => a.Status == "IN_REPAIR");
                model.RetiredAssets = await _context.Assets.CountAsync(a => a.Status == "RETIRED");
            }

            if (User.IsInRole("PURCHASING") || User.IsInRole("IT_SERVICE") || User.IsInRole("IT_SUPPORT"))
            {
                model.TotalReplacementRequests = await _context.ReplacementRequests.CountAsync();
                model.PendingReplacementRequests = await _context.ReplacementRequests.CountAsync(r => r.Status == "PENDING");
            }

            return View(model);
        }
    }
}
