using Himam_main.Data;
using Himam_main.Models;
using Himam_main.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly HimanAlhayahContext _context;

        public HomeController(HimanAlhayahContext context)
        {
            _context = context;
        }

        // GET: /User/Home/Index  (الصفحة الرئيسية)
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                HeroSection = await _context.HeroSections
                    .Where(h => h.IsVisible)
                    .OrderBy(h => h.SortOrder)
                    .FirstOrDefaultAsync(),
                AboutSection = await _context.AboutSections
                    .Where(a => a.IsVisible)
                    .OrderBy(a => a.SortOrder)
                    .FirstOrDefaultAsync(),
                Sectors = await _context.Sectors
                    .Where(s => s.IsVisible)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync(),
                CompanyValues = await _context.CompanyValues
                    .Where(c => c.IsVisible)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync(),
                ProcessSteps = await _context.ProcessSteps
                    .Where(p => p.IsVisible)
                    .OrderBy(p => p.SortOrder)
                    .ToListAsync(),
                ContactInfo = await _context.ContactInfos
                    .Where(c => c.IsVisible)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync(),
                SocialMediaLinks = await _context.SocialMediaLinks
                    .Where(s => s.IsVisible)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync(),
                StatItems = await _context.StatItems
                    .Where(s => s.IsVisible)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: /User/Home/About  (من نحن)
        public async Task<IActionResult> About()
        {
            var viewModel = new AboutViewModel
            {
                CompanyValues = await _context.CompanyValues
                    .Where(c => c.IsVisible)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync(),
                ProcessSteps = await _context.ProcessSteps
                    .Where(p => p.IsVisible)
                    .OrderBy(p => p.SortOrder)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: /User/Home/Contact  (تواصل معنا)
        public async Task<IActionResult> Contact()
        {
            var viewModel = new ContactViewModel
            {
                ContactInfo = await _context.ContactInfos
                    .Where(c => c.IsVisible)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync(),
                SocialMediaLinks = await _context.SocialMediaLinks
                    .Where(s => s.IsVisible)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: /User/Home/News  (أخبار وفعاليات)
        public async Task<IActionResult> News()
        {
            var news = await _context.News
                .Where(n => n.Status == "Published")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(news);
        }

        // GET: /User/Home/NewsSingle/5  (تفاصيل خبر/فعالية واحدة)
        public async Task<IActionResult> NewsSingle(int id)
        {
            var newsItem = await _context.News
                .Include(n => n.User)
                .Include(n => n.Comments)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (newsItem == null)
            {
                return NotFound();
            }

            return View(newsItem);
        }
    }
}
