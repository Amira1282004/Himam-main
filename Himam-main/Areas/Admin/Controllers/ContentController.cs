using System.Security.Claims;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ContentController : Controller
    {
        private readonly HimanAlhayahContext _context;

        public ContentController(HimanAlhayahContext context)
        {
            _context = context;
        }

        // Hero Section Management
        public async Task<IActionResult> HeroSection()
        {
            var heroSection = await _context.HeroSections
                .Include(h => h.User)
                .FirstOrDefaultAsync();

            if (heroSection == null)
            {
                // Create default hero section if not exists
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                heroSection = new HeroSection
                {
                    Title = "نصنع تجارب ترفيهية لأن جودة الحياة حق للجميع",
                    Subtitle = "صناعة الفعاليات وتطوير التجارب",
                    CtaText = "استكشف خدماتنا",
                    CtaLink = "#sectors",
                    YoutubeVideoId = "c8bBHCEI9AE",
                    IsVideoEnabled = true,
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = userId
                };
                _context.HeroSections.Add(heroSection);
                await _context.SaveChangesAsync();
            }

            return View(heroSection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HeroSection(HeroSection model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.HeroSections.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Title = model.Title;
                existing.Subtitle = model.Subtitle;
                existing.CtaText = model.CtaText;
                existing.CtaLink = model.CtaLink;
                existing.YoutubeVideoId = model.YoutubeVideoId;
                existing.IsVideoEnabled = model.IsVideoEnabled;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                // Log the change
                await LogChange("HeroSection", "تعديل قسم Hero", $"تعديل قسم Hero: {model.Title}");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث قسم Hero بنجاح";
            return RedirectToAction(nameof(HeroSection));
        }

        // About Section Management
        public async Task<IActionResult> AboutSection()
        {
            var aboutSection = await _context.AboutSections
                .Include(a => a.User)
                .FirstOrDefaultAsync();

            if (aboutSection == null)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                aboutSection = new AboutSection
                {
                    Eyebrow = "ما نقوم به",
                    Title = "تنطلق همم الحياة من همة تسعى إلى تحويل الأفكار والرسائل والأهداف إلى تجارب حقيقية ذات أثر ملموس",
                    Description = "نجمع بين التفكير الاستراتيجي والإبداع والتنفيذ المنضبط؛ لنقدم تجارب مترابطة تضع الإنسان في جوهرها، وتنتقل بالفكرة من التصور إلى واقع يُعاش ويُتذكر.",
                    AdditionalDescription = "ولا تنتهي قيمة التجربة بانتهاء تنفيذها، بل تمتد إلى ما تحققه من نتائج، وما تتركه من أثر يمكن قياسه وتطويره والبناء عليه.",
                    ChairmanName = "ظافر الشهراني",
                    ChairmanTitle = "رئيس مجلس الإدارة",
                    ChairmanImage = "~/assets/chairman-dhafer-alshahrani.png",
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = userId
                };
                _context.AboutSections.Add(aboutSection);
                await _context.SaveChangesAsync();
            }

            return View(aboutSection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AboutSection(AboutSection model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.AboutSections.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Eyebrow = model.Eyebrow;
                existing.Title = model.Title;
                existing.Description = model.Description;
                existing.AdditionalDescription = model.AdditionalDescription;
                existing.ChairmanName = model.ChairmanName;
                existing.ChairmanTitle = model.ChairmanTitle;
                existing.ChairmanImage = model.ChairmanImage;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                await LogChange("AboutSection", "تعديل قسم About", $"تعديل قسم About: {model.Title}");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث قسم About بنجاح";
            return RedirectToAction(nameof(AboutSection));
        }

        // Sectors Management
        public async Task<IActionResult> Sectors()
        {
            var sectors = await _context.Sectors
                .Include(s => s.User)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return View(sectors);
        }

        public async Task<IActionResult> CreateSector()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSector(Sector model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UserId = userId;
            model.IsVisible = true;

            _context.Sectors.Add(model);
            await _context.SaveChangesAsync();

            await LogChange("Sector", "إنشاء قطاع جديد", $"إنشاء قطاع: {model.Title}");

            TempData["SuccessMessage"] = "تم إنشاء القطاع بنجاح";
            return RedirectToAction(nameof(Sectors));
        }

        public async Task<IActionResult> EditSector(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
                return NotFound();

            return View(sector);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSector(Sector model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.Sectors.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Title = model.Title;
                existing.Description = model.Description;
                existing.Image = model.Image;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                await LogChange("Sector", "تعديل قطاع", $"تعديل قطاع: {model.Title}");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث القطاع بنجاح";
            return RedirectToAction(nameof(Sectors));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSector(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector != null)
            {
                _context.Sectors.Remove(sector);
                await _context.SaveChangesAsync();

                await LogChange("Sector", "حذف قطاع", $"حذف قطاع: {sector.Title}");

                TempData["SuccessMessage"] = "تم حذف القطاع بنجاح";
            }

            return RedirectToAction(nameof(Sectors));
        }

        // Company Values Management
        public async Task<IActionResult> CompanyValues()
        {
            var values = await _context.CompanyValues
                .Include(v => v.User)
                .OrderBy(v => v.SortOrder)
                .ToListAsync();

            return View(values);
        }

        public async Task<IActionResult> CreateCompanyValue()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompanyValue(CompanyValue model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UserId = userId;
            model.IsVisible = true;

            _context.CompanyValues.Add(model);
            await _context.SaveChangesAsync();

            await LogChange("CompanyValue", "إنشاء قيمة جديدة", $"إنشاء قيمة: {model.Title}");

            TempData["SuccessMessage"] = "تم إنشاء القيمة بنجاح";
            return RedirectToAction(nameof(CompanyValues));
        }

        public async Task<IActionResult> EditCompanyValue(int id)
        {
            var value = await _context.CompanyValues.FindAsync(id);
            if (value == null)
                return NotFound();

            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompanyValue(CompanyValue model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.CompanyValues.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Title = model.Title;
                existing.Content = model.Content;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                await LogChange("CompanyValue", "تعديل قيمة", $"تعديل قيمة: {model.Title}");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث القيمة بنجاح";
            return RedirectToAction(nameof(CompanyValues));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyValue(int id)
        {
            var value = await _context.CompanyValues.FindAsync(id);
            if (value != null)
            {
                _context.CompanyValues.Remove(value);
                await _context.SaveChangesAsync();

                await LogChange("CompanyValue", "حذف قيمة", $"حذف قيمة: {value.Title}");

                TempData["SuccessMessage"] = "تم حذف القيمة بنجاح";
            }

            return RedirectToAction(nameof(CompanyValues));
        }

        // Contact Info Management
        public async Task<IActionResult> ContactInfo()
        {
            var contactInfo = await _context.ContactInfos
                .Include(c => c.User)
                .FirstOrDefaultAsync();

            if (contactInfo == null)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                contactInfo = new ContactInfo
                {
                    Email = "info@himamalhayah.sa",
                    Phone = "0535105327",
                    Address = "جدة، حي مشرفة شارع عين الوهيط مبني 3654 الرمز البريدي23332",
                    WorkingHours = "الأحد – الخميس، 9 صباحًا – 5 مساءً",
                    MapEmbedUrl = "https://www.google.com/maps/embed?...",
                    IsVisible = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = userId
                };
                _context.ContactInfos.Add(contactInfo);
                await _context.SaveChangesAsync();
            }

            return View(contactInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactInfo(ContactInfo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.ContactInfos.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Email = model.Email;
                existing.Phone = model.Phone;
                existing.Address = model.Address;
                existing.WorkingHours = model.WorkingHours;
                existing.MapEmbedUrl = model.MapEmbedUrl;
                existing.IsVisible = model.IsVisible;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                await LogChange("ContactInfo", "تعديل معلومات التواصل", "تعديل معلومات التواصل");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث معلومات التواصل بنجاح";
            return RedirectToAction(nameof(ContactInfo));
        }

        // Social Media Links Management
        public async Task<IActionResult> SocialMediaLinks()
        {
            var links = await _context.SocialMediaLinks
                .Include(l => l.User)
                .OrderBy(l => l.SortOrder)
                .ToListAsync();

            return View(links);
        }

        public async Task<IActionResult> CreateSocialMediaLink()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSocialMediaLink(SocialMediaLink model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UserId = userId;
            model.IsVisible = true;

            _context.SocialMediaLinks.Add(model);
            await _context.SaveChangesAsync();

            await LogChange("SocialMediaLink", "إنشاء رابط اجتماعي جديد", $"إنشاء رابط: {model.Platform}");

            TempData["SuccessMessage"] = "تم إنشاء الرابط الاجتماعي بنجاح";
            return RedirectToAction(nameof(SocialMediaLinks));
        }

        public async Task<IActionResult> EditSocialMediaLink(int id)
        {
            var link = await _context.SocialMediaLinks.FindAsync(id);
            if (link == null)
                return NotFound();

            return View(link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSocialMediaLink(SocialMediaLink model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.SocialMediaLinks.FindAsync(model.Id);

            if (existing != null)
            {
                existing.Platform = model.Platform;
                existing.Url = model.Url;
                existing.IconSvg = model.IconSvg;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                await LogChange("SocialMediaLink", "تعديل رابط اجتماعي", $"تعديل رابط: {model.Platform}");

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث الرابط الاجتماعي بنجاح";
            return RedirectToAction(nameof(SocialMediaLinks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSocialMediaLink(int id)
        {
            var link = await _context.SocialMediaLinks.FindAsync(id);
            if (link != null)
            {
                _context.SocialMediaLinks.Remove(link);
                await _context.SaveChangesAsync();

                await LogChange("SocialMediaLink", "حذف رابط اجتماعي", $"حذف رابط: {link.Platform}");

                TempData["SuccessMessage"] = "تم حذف الرابط الاجتماعي بنجاح";
            }

            return RedirectToAction(nameof(SocialMediaLinks));
        }

        // Audit Log Management
        public async Task<IActionResult> AuditLogs()
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(100)
                .ToListAsync();

            return View(logs);
        }

        private async Task LogChange(string entityType, string action, string details)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = $"{entityType} - {action}",
                Details = details,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
