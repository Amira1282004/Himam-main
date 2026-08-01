using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string loginEmail, string loginPassword, bool rememberMe = false)
        {

            return RedirectToAction("Index", "Dashboard");
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
    }
}