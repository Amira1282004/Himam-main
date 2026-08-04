using Himam_main.Authorization;
using Himam_main.Models;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManagePages)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class PagesController : Controller
{
    private readonly IContentService _content;

    public PagesController(IContentService content)
    {
        _content = content;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pages = await _content.GetPagesAsync();
        return Json(pages.Select(p => new
        {
            p.Id,
            p.Title,
            p.Slug,
            p.Status,
            p.MetaTitle,
            p.MetaDescription,
            p.ContentAr,
            p.UpdatedAt
        }));
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug)
    {
        var page = await _content.GetPageBySlugAsync(slug);
        if (page is null)
            return NotFound();
        return Json(page);
    }

    [HttpPost("{slug}")]
    public async Task<IActionResult> Save(string slug, [FromBody] PageInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var page = await _content.SavePageAsync(slug, new Page
        {
            Title = input.Title,
            ContentAr = input.ContentAr,
            ContentEn = input.ContentEn,
            MetaTitle = input.MetaTitle,
            MetaDescription = input.MetaDescription,
            Image = input.Image,
            Status = input.Status,
            SortOrder = input.SortOrder,
            IsVisible = input.IsVisible
        }, userId.Value, User.CanPublish());

        return Json(new { success = true, page.Id, page.Status });
    }

    [HttpPatch("{slug}/toggle-visibility")]
    public async Task<IActionResult> ToggleVisibility(string slug)
    {
        var page = await _content.GetPageBySlugAsync(slug);
        if (page is null)
            return NotFound();

        page.IsVisible = !page.IsVisible;
        page.UpdatedAt = DateTime.Now;
        await _content.UpdatePageAsync(page);

        return Json(new { success = true, isVisible = page.IsVisible });
    }

    public class PageInput
    {
        public string? Title { get; set; }
        public string? ContentAr { get; set; }
        public string? ContentEn { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Image { get; set; }
        public string? Status { get; set; }
        public int? SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
