using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.Controllers
{
    [Authorize(Roles = "IT_SERVICE,IT_SUPPORT,PURCHASING")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? entityName, int pageIndex = 1)
        {
            var query = _context.AuditLogs
                .Include(a => a.PerformedBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }

            query = query.OrderByDescending(a => a.PerformedAt);

            var result = await PaginatedList<Models.Entities.AuditLog>.CreateAsync(query, pageIndex, 20);

            ViewBag.EntityName = entityName;
            return View(result);
        }
    }
}
