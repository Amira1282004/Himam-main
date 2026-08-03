using System.Security.Claims;
using Himam_main.Data;
using Himam_main.Models;
using Himam_main.Services;
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
        private readonly IAuditLogService _auditLogService;

        public ContentController(HimanAlhayahContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
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

                var oldValues = new
                {
                    existing.Title,
                    existing.Subtitle,
                    existing.CtaText,
                    existing.CtaLink,
                    existing.YoutubeVideoId,
                    existing.IsVideoEnabled,
                    existing.IsVisible,
                    existing.SortOrder
                };

                var newValues = new
                {
                    model.Title,
                    model.Subtitle,
                    model.CtaText,
                    model.CtaLink,
                    model.YoutubeVideoId,
                    model.IsVideoEnabled,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("HeroSection", "تعديل قسم Hero", $"تعديل قسم Hero: {model.Title}", oldValues, newValues, model.Id);

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

                var oldValues = new
                {
                    existing.Eyebrow,
                    existing.Title,
                    existing.Description,
                    existing.AdditionalDescription,
                    existing.ChairmanName,
                    existing.ChairmanTitle,
                    existing.ChairmanImage,
                    existing.IsVisible,
                    existing.SortOrder
                };

                var newValues = new
                {
                    model.Eyebrow,
                    model.Title,
                    model.Description,
                    model.AdditionalDescription,
                    model.ChairmanName,
                    model.ChairmanTitle,
                    model.ChairmanImage,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("AboutSection", "تعديل قسم About", $"تعديل قسم About: {model.Title}", oldValues, newValues, model.Id);

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

            var newValues = new
            {
                model.Title,
                model.Description,
                model.Image,
                model.IsVisible,
                model.SortOrder
            };

            await LogChange("Sector", "إنشاء قطاع جديد", $"إنشاء قطاع: {model.Title}", null, newValues, model.Id);

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
                var oldValues = new
                {
                    existing.Title,
                    existing.Description,
                    existing.Image,
                    existing.IsVisible,
                    existing.SortOrder
                };

                existing.Title = model.Title;
                existing.Description = model.Description;
                existing.Image = model.Image;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                var newValues = new
                {
                    model.Title,
                    model.Description,
                    model.Image,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("Sector", "تعديل قطاع", $"تعديل قطاع: {model.Title}", oldValues, newValues, model.Id);

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
                var oldValues = new
                {
                    sector.Title,
                    sector.Description,
                    sector.Image,
                    sector.IsVisible,
                    sector.SortOrder
                };

                _context.Sectors.Remove(sector);
                await _context.SaveChangesAsync();

                await LogChange("Sector", "حذف قطاع", $"حذف قطاع: {sector.Title}", oldValues, null, sector.Id);

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

            var newValues = new
            {
                model.Title,
                model.Content,
                model.IsVisible,
                model.SortOrder
            };

            await LogChange("CompanyValue", "إنشاء قيمة جديدة", $"إنشاء قيمة: {model.Title}", null, newValues, model.Id);

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

                var oldValues = new
                {
                    existing.Title,
                    existing.Content,
                    existing.IsVisible,
                    existing.SortOrder
                };

                var newValues = new
                {
                    model.Title,
                    model.Content,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("CompanyValue", "تعديل قيمة", $"تعديل قيمة: {model.Title}", oldValues, newValues, model.Id);

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
                var oldValues = new
                {
                    value.Title,
                    value.Content,
                    value.IsVisible,
                    value.SortOrder
                };

                _context.CompanyValues.Remove(value);
                await _context.SaveChangesAsync();

                await LogChange("CompanyValue", "حذف قيمة", $"حذف قيمة: {value.Title}", oldValues, null, value.Id);

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

                var oldValues = new
                {
                    existing.Email,
                    existing.Phone,
                    existing.Address,
                    existing.WorkingHours,
                    existing.MapEmbedUrl,
                    existing.IsVisible,
                    existing.SortOrder
                };

                var newValues = new
                {
                    model.Email,
                    model.Phone,
                    model.Address,
                    model.WorkingHours,
                    model.MapEmbedUrl,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("ContactInfo", "تعديل معلومات التواصل", "تعديل معلومات التواصل", oldValues, newValues, model.Id);

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

            var newValues = new
            {
                model.Platform,
                model.PlatformName,
                model.Url,
                model.IconSvg,
                model.IsVisible,
                model.SortOrder
            };

            await LogChange("SocialMediaLink", "إنشاء رابط اجتماعي جديد", $"إنشاء رابط: {model.Platform}", null, newValues, model.Id);

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

                var oldValues = new
                {
                    existing.Platform,
                    existing.PlatformName,
                    existing.Url,
                    existing.IconSvg,
                    existing.IsVisible,
                    existing.SortOrder
                };

                var newValues = new
                {
                    model.Platform,
                    model.PlatformName,
                    model.Url,
                    model.IconSvg,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("SocialMediaLink", "تعديل رابط اجتماعي", $"تعديل رابط: {model.Platform}", oldValues, newValues, model.Id);

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
                var oldValues = new
                {
                    link.Platform,
                    link.PlatformName,
                    link.Url,
                    link.IconSvg,
                    link.IsVisible,
                    link.SortOrder
                };

                _context.SocialMediaLinks.Remove(link);
                await _context.SaveChangesAsync();

                await LogChange("SocialMediaLink", "حذف رابط اجتماعي", $"حذف رابط: {link.Platform}", oldValues, null, link.Id);

                TempData["SuccessMessage"] = "تم حذف الرابط الاجتماعي بنجاح";
            }

            return RedirectToAction(nameof(SocialMediaLinks));
        }

        // Process Steps Management
        public async Task<IActionResult> ProcessSteps()
        {
            var steps = await _context.ProcessSteps
                .Include(p => p.User)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            return View(steps);
        }

        public async Task<IActionResult> CreateProcessStep()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProcessStep(ProcessStep model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UserId = userId;
            model.IsVisible = true;

            _context.ProcessSteps.Add(model);
            await _context.SaveChangesAsync();

            var newValues = new
            {
                model.Title,
                model.Subtitle,
                model.StepNumber,
                model.IsVisible,
                model.SortOrder
            };

            await LogChange("ProcessStep", "إنشاء خطوة جديدة", $"إنشاء خطوة: {model.Title}", null, newValues, model.Id);

            TempData["SuccessMessage"] = "تم إنشاء الخطوة بنجاح";
            return RedirectToAction(nameof(ProcessSteps));
        }

        public async Task<IActionResult> EditProcessStep(int id)
        {
            var step = await _context.ProcessSteps.FindAsync(id);
            if (step == null)
                return NotFound();

            return View(step);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProcessStep(ProcessStep model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.ProcessSteps.FindAsync(model.Id);

            if (existing != null)
            {
                var oldValues = new
                {
                    existing.Title,
                    existing.Subtitle,
                    existing.StepNumber,
                    existing.IsVisible,
                    existing.SortOrder
                };

                existing.Title = model.Title;
                existing.Subtitle = model.Subtitle;
                existing.StepNumber = model.StepNumber;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                var newValues = new
                {
                    model.Title,
                    model.Subtitle,
                    model.StepNumber,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("ProcessStep", "تعديل خطوة", $"تعديل خطوة: {model.Title}", oldValues, newValues, model.Id);

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث الخطوة بنجاح";
            return RedirectToAction(nameof(ProcessSteps));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProcessStep(int id)
        {
            var step = await _context.ProcessSteps.FindAsync(id);
            if (step != null)
            {
                var oldValues = new
                {
                    step.Title,
                    step.Subtitle,
                    step.StepNumber,
                    step.IsVisible,
                    step.SortOrder
                };

                _context.ProcessSteps.Remove(step);
                await _context.SaveChangesAsync();

                await LogChange("ProcessStep", "حذف خطوة", $"حذف خطوة: {step.Title}", oldValues, null, step.Id);

                TempData["SuccessMessage"] = "تم حذف الخطوة بنجاح";
            }

            return RedirectToAction(nameof(ProcessSteps));
        }

        // Stat Items Management
        public async Task<IActionResult> StatItems()
        {
            var stats = await _context.StatItems
                .Include(s => s.User)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return View(stats);
        }

        public async Task<IActionResult> CreateStatItem()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStatItem(StatItem model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UserId = userId;
            model.IsVisible = true;

            _context.StatItems.Add(model);
            await _context.SaveChangesAsync();

            var newValues = new
            {
                model.Title,
                model.Value,
                model.Description,
                model.IsVisible,
                model.SortOrder
            };

            await LogChange("StatItem", "إنشاء إحصائية جديدة", $"إنشاء إحصائية: {model.Title}", null, newValues, model.Id);

            TempData["SuccessMessage"] = "تم إنشاء الإحصائية بنجاح";
            return RedirectToAction(nameof(StatItems));
        }

        public async Task<IActionResult> EditStatItem(int id)
        {
            var stat = await _context.StatItems.FindAsync(id);
            if (stat == null)
                return NotFound();

            return View(stat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatItem(StatItem model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.StatItems.FindAsync(model.Id);

            if (existing != null)
            {
                var oldValues = new
                {
                    existing.Title,
                    existing.Value,
                    existing.Description,
                    existing.IsVisible,
                    existing.SortOrder
                };

                existing.Title = model.Title;
                existing.Value = model.Value;
                existing.Description = model.Description;
                existing.IsVisible = model.IsVisible;
                existing.SortOrder = model.SortOrder;
                existing.UpdatedAt = DateTime.Now;
                existing.UserId = userId;

                var newValues = new
                {
                    model.Title,
                    model.Value,
                    model.Description,
                    model.IsVisible,
                    model.SortOrder
                };

                await LogChange("StatItem", "تعديل إحصائية", $"تعديل إحصائية: {model.Title}", oldValues, newValues, model.Id);

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث الإحصائية بنجاح";
            return RedirectToAction(nameof(StatItems));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStatItem(int id)
        {
            var stat = await _context.StatItems.FindAsync(id);
            if (stat != null)
            {
                var oldValues = new
                {
                    stat.Title,
                    stat.Value,
                    stat.Description,
                    stat.IsVisible,
                    stat.SortOrder
                };

                _context.StatItems.Remove(stat);
                await _context.SaveChangesAsync();

                await LogChange("StatItem", "حذف إحصائية", $"حذف إحصائية: {stat.Title}", oldValues, null, stat.Id);

                TempData["SuccessMessage"] = "تم حذف الإحصائية بنجاح";
            }

            return RedirectToAction(nameof(StatItems));
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

        private async Task LogChange(string entityType, string action, string details, object? oldValues = null, object? newValues = null, int? entityId = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            await _auditLogService.LogEntityChangeAsync(
                entityName: entityType,
                actionType: action,
                userId: userId,
                entityId: entityId,
                oldValues: oldValues,
                newValues: newValues,
                httpContext: HttpContext
            );
        }
    }
}
