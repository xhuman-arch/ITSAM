using ITServiceAssetManagement.Data;
using ITServiceAssetManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.ViewComponents
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationBadgeViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (UserClaimsPrincipal?.Identity == null || !UserClaimsPrincipal.Identity.IsAuthenticated)
            {
                return Content(string.Empty);
            }

            var userId = _userManager.GetUserId(UserClaimsPrincipal);
            var unreadCount = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return View("Default", unreadCount);
        }
    }
}
