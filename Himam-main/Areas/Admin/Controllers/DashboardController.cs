using System.Security.Claims;
using Himam_main.Data;
using Himam_main.Helpers;
using Himam_main.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly HimanAlhayahContext _context;

        public DashboardController(HimanAlhayahContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var currentUser = await _context.Users
                .Include(u => u.Roles)
                .FirstAsync(u => u.Id == userId);

            var primaryRole = currentUser.Roles.FirstOrDefault()?.Name ?? "Super Admin";
            var roleDisplay = UserDisplayHelper.GetRoleDisplay(primaryRole);

            var allUsers = await _context.Users
                .Include(u => u.Roles)
                .OrderBy(u => u.Id)
                .ToListAsync();

            var usersByRole = await _context.Roles
                .Select(r => new { r.Name, Count = r.Users.Count })
                .ToDictionaryAsync(x => x.Name, x => x.Count);

            var model = new DashboardViewModel
            {
                CurrentUser = new CurrentUserViewModel
                {
                    Id = currentUser.Id,
                    Username = currentUser.Username,
                    Email = currentUser.Email,
                    RoleName = primaryRole,
                    RoleNameAr = roleDisplay.Ar,
                    RoleBadgeClass = roleDisplay.BadgeClass,
                    Initials = UserDisplayHelper.GetInitials(currentUser.Username),
                    CreatedAt = currentUser.CreatedAt
                },
                TotalUsers = allUsers.Count,
                UsersByRole = usersByRole,
                Users = allUsers.Select(u =>
                {
                    var role = u.Roles.FirstOrDefault()?.Name ?? "Unknown";
                    var display = UserDisplayHelper.GetRoleDisplay(role);
                    return new UserListItemViewModel
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        RoleName = role,
                        RoleNameAr = display.Ar,
                        RoleBadgeClass = display.BadgeClass,
                        Initials = UserDisplayHelper.GetInitials(u.Username),
                        CreatedAt = u.CreatedAt,
                        IsCurrentUser = u.Id == userId
                    };
                }).ToList(),
                SuccessMessage = TempData["SuccessMessage"] as string
            };

            return View(model);
        }
    }
}
