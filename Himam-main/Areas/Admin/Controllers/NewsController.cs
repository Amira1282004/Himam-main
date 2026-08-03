using Himam_main.Authorization;
using Himam_main.Models;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.EditContent)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class NewsController : Controller
{
    private readonly IContentService _content;

    public NewsController(IContentService content)
    {
        _content = content;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status = null)
    {
        var items = await _content.GetNewsAsync(status);
        return Json(items.Select(n => new
        {
            n.Id,
            n.Title,
            n.Slug,
            n.Status,
            n.MetaTitle,
            n.UpdatedAt,
            n.CreatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var news = await _content.GetNewsByIdAsync(id);
        if (news is null)
            return NotFound();
        return Json(news);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NewsInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var news = await _content.SaveNewsAsync(MapInput(input), userId.Value, User.CanPublish());
        return Json(new { success = true, news.Id, news.Status });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] NewsInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var news = await _content.SaveNewsAsync(MapInput(input), userId.Value, User.CanPublish(), id);
        return Json(new { success = true, news.Id, news.Status });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.ManagePages)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var ok = await _content.DeleteNewsAsync(id, userId.Value);
        return ok ? Json(new { success = true }) : NotFound();
    }

    private static News MapInput(NewsInput input) => new()
    {
        Title = input.Title,
        Slug = input.Slug,
        ContentAr = input.ContentAr,
        ContentEn = input.ContentEn,
        Image = input.Image,
        MetaTitle = input.MetaTitle,
        MetaDescription = input.MetaDescription,
        Status = input.Status
    };

    public class NewsInput
    {
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? ContentAr { get; set; }
        public string? ContentEn { get; set; }
        public string? Image { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Status { get; set; }
    }
}
