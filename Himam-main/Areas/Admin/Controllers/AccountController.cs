using System.Security.Claims;
using Himam_main.Data;
using Himam_main.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly HimanAlhayahContext _context;

        public AccountController(HimanAlhayahContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginEmail, string loginPassword, bool rememberMe = false)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == loginEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginPassword, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                return View();
            }

            await SignInUserAsync(user, rememberMe);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileEditViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (model.Id != userId)
                return Forbid();

            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword != model.ConfirmPassword)
            {
                TempData["ProfileError"] = "كلمتا المرور غير متطابقتين";
                return RedirectToAction("Index", "Dashboard");
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var oldUsername = user.Username;

            if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != userId))
            {
                TempData["ProfileError"] = "البريد الإلكتروني مستخدم بالفعل";
                return RedirectToAction("Index", "Dashboard");
            }

            user.Username = model.Username.Trim();
            user.Email = model.Email.Trim();
            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(t => t.Name == oldUsername);

            if (teamMember != null)
            {
                teamMember.Name = user.Username;
                teamMember.Position = user.Roles.FirstOrDefault()?.Name switch
                {
                    "Super Admin" => "المدير العام",
                    "Site Manager" => "مدير الموقع",
                    "Content Editor" => "محرر المحتوى",
                    "Customer Service" => "خدمة العملاء",
                    _ => teamMember.Position
                };
            }

            await _context.SaveChangesAsync();
            await SignInUserAsync(user, isPersistent: true);

            TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح";
            TempData["OpenProfile"] = false;
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(string fullname, string email, string phone, string role, string password, string password2)
        {
            if (password != password2)
            {
                ModelState.AddModelError(string.Empty, "كلمتا المرور غير متطابقتين");
                return View();
            }

            return RedirectToAction(nameof(Login));
        }

        private async Task SignInUserAsync(Models.User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(30) : null
                });
        }
    }
}
