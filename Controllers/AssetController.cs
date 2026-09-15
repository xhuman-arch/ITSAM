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
    [Authorize(Roles = "IT_SERVICE,IT_SUPPORT")]
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;

        public AssetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAuditLogService auditLog)
        {
            _context = context;
            _userManager = userManager;
            _auditLog = auditLog;
        }

        // GET: Asset
        public async Task<IActionResult> Index(string? status, string? category, string? search, int pageIndex = 1)
        {
            var query = _context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(a => a.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.AssetName.Contains(search) || (a.AssetCode != null && a.AssetCode.Contains(search)));
            }

            query = query.OrderByDescending(a => a.CreatedAt);

            var result = await PaginatedList<Asset>.CreateAsync(query, pageIndex, 10);

            ViewBag.Status = status;
            ViewBag.Category = category;
            ViewBag.Search = search;

            return View(result);
        }

        // GET: Asset/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Tickets)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null) return NotFound();
            return View(asset);
        }

        // GET: Asset/Create
        public IActionResult Create()
        {
            return View(new AssetFormViewModel());
        }

        // POST: Asset/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;

            var asset = new Asset
            {
                AssetName = model.AssetName,
                AssetCode = model.AssetCode,
                Category = model.Category,
                Location = model.Location,
                Status = model.Status,
                PurchaseDate = model.PurchaseDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync("Asset", asset.Id, "CREATE", $"Asset '{asset.AssetName}' ditambahkan.", userId);

            TempData["Success"] = "Asset berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Asset/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            var model = new AssetFormViewModel
            {
                Id = asset.Id,
                AssetName = asset.AssetName,
                AssetCode = asset.AssetCode,
                Category = asset.Category,
                Location = asset.Location,
                Status = asset.Status,
                PurchaseDate = asset.PurchaseDate
            };

            return View(model);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssetFormViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var oldStatus = asset.Status;

            asset.AssetName = model.AssetName;
            asset.AssetCode = model.AssetCode;
            asset.Category = model.Category;
            asset.Location = model.Location;
            asset.Status = model.Status;
            asset.PurchaseDate = model.PurchaseDate;

            await _context.SaveChangesAsync();

            var description = oldStatus != model.Status
                ? $"Asset '{asset.AssetName}' diperbarui. Status berubah dari {oldStatus} ke {model.Status}."
                : $"Asset '{asset.AssetName}' diperbarui.";

            await _auditLog.LogAsync("Asset", asset.Id, "UPDATE", description, userId);

            TempData["Success"] = "Asset berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Asset/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if (asset == null) return NotFound();
            return View(asset);
        }

        // POST: Asset/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                var userId = _userManager.GetUserId(User)!;
                var assetName = asset.AssetName;

                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();

                await _auditLog.LogAsync("Asset", id, "DELETE", $"Asset '{assetName}' dihapus.", userId);

                TempData["Success"] = "Asset berhasil dihapus.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
