using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Himam_main;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.EditContent)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class SeoSettingsController : Controller
{
    private readonly HimanAlhayahContext _context;

    public SeoSettingsController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _context.SeoSettings
            .Include(s => s.User)
            .OrderBy(s => s.PageName)
            .ToListAsync();
        
        return Json(items.Select(s => new
        {
            s.Id,
            s.PageName,
            s.MetaTitle,
            s.IsEnabled,
            s.CreatedAt,
            s.UpdatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var setting = await _context.SeoSettings
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (setting is null)
            return NotFound();
        
        return Json(setting);
    }

    [HttpGet("by-page/{pageName}")]
    public async Task<IActionResult> GetByPage(string pageName)
    {
        var setting = await _context.SeoSettings
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.PageName == pageName);
        
        if (setting is null)
            return NotFound();
        
        return Json(setting);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SeoSettingInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var setting = new SeoSetting
        {
            PageName = input.PageName,
            MetaTitle = input.MetaTitle,
            MetaDescription = input.MetaDescription,
            MetaKeywords = input.MetaKeywords,
            CanonicalUrl = input.CanonicalUrl,
            OgTitle = input.OgTitle,
            OgDescription = input.OgDescription,
            OgImage = input.OgImage,
            IsEnabled = input.IsEnabled,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserId = userId.Value
        };

        _context.SeoSettings.Add(setting);
        await _context.SaveChangesAsync();

        return Json(new { success = true, setting.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SeoSettingInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var setting = await _context.SeoSettings.FindAsync(id);
        if (setting is null)
            return NotFound();

        setting.PageName = input.PageName;
        setting.MetaTitle = input.MetaTitle;
        setting.MetaDescription = input.MetaDescription;
        setting.MetaKeywords = input.MetaKeywords;
        setting.CanonicalUrl = input.CanonicalUrl;
        setting.OgTitle = input.OgTitle;
        setting.OgDescription = input.OgDescription;
        setting.OgImage = input.OgImage;
        setting.IsEnabled = input.IsEnabled;
        setting.UpdatedAt = DateTime.Now;
        setting.UserId = userId.Value;

        await _context.SaveChangesAsync();

        return Json(new { success = true, setting.Id });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.ManagePages)]
    public async Task<IActionResult> Delete(int id)
    {
        var setting = await _context.SeoSettings.FindAsync(id);
        if (setting is null)
            return NotFound();

        _context.SeoSettings.Remove(setting);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPatch("{id:int}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var setting = await _context.SeoSettings.FindAsync(id);
        if (setting is null)
            return NotFound();

        setting.IsEnabled = !setting.IsEnabled;
        setting.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isEnabled = setting.IsEnabled });
    }

    public class SeoSettingInput
    {
        public string PageName { get; set; } = string.Empty;
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string MetaKeywords { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string OgTitle { get; set; } = string.Empty;
        public string OgDescription { get; set; } = string.Empty;
        public string OgImage { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }
}