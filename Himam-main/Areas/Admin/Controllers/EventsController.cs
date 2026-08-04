using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Himam_main.Services;
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
public class EventsController : Controller
{
    private readonly HimanAlhayahContext _context;

    public EventsController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _context.Events
            .Include(e => e.User)
            .OrderBy(e => e.SortOrder)
            .ThenByDescending(e => e.EventDate)
            .ToListAsync();
        
        return Json(items.Select(e => new
        {
            e.Id,
            e.Title,
            e.Slug,
            e.Status,
            e.EventDate,
            e.Location,
            e.IsFeatured,
            e.IsVisible,
            e.SortOrder,
            e.CreatedAt,
            e.UpdatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (eventItem is null)
            return NotFound();
        
        return Json(eventItem);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var eventItem = new Event
        {
            Title = input.Title,
            Slug = input.Slug,
            DescriptionAr = input.DescriptionAr,
            DescriptionEn = input.DescriptionEn,
            Image = input.Image,
            EventDate = input.EventDate,
            Location = input.Location,
            MetaTitle = input.MetaTitle,
            MetaDescription = input.MetaDescription,
            Status = input.Status,
            IsFeatured = input.IsFeatured,
            SortOrder = input.SortOrder,
            IsVisible = input.IsVisible,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserId = userId.Value
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return Json(new { success = true, eventItem.Id, eventItem.Status });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
            return NotFound();

        eventItem.Title = input.Title;
        eventItem.Slug = input.Slug;
        eventItem.DescriptionAr = input.DescriptionAr;
        eventItem.DescriptionEn = input.DescriptionEn;
        eventItem.Image = input.Image;
        eventItem.EventDate = input.EventDate;
        eventItem.Location = input.Location;
        eventItem.MetaTitle = input.MetaTitle;
        eventItem.MetaDescription = input.MetaDescription;
        eventItem.Status = input.Status;
        eventItem.IsFeatured = input.IsFeatured;
        eventItem.SortOrder = input.SortOrder;
        eventItem.IsVisible = input.IsVisible;
        eventItem.UpdatedAt = DateTime.Now;
        eventItem.UserId = userId.Value;

        await _context.SaveChangesAsync();

        return Json(new { success = true, eventItem.Id, eventItem.Status });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.ManagePages)]
    public async Task<IActionResult> Delete(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
            return NotFound();

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPatch("{id:int}/toggle-visibility")]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
            return NotFound();

        eventItem.IsVisible = !eventItem.IsVisible;
        eventItem.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isVisible = eventItem.IsVisible });
    }

    [HttpPatch("{id:int}/toggle-featured")]
    public async Task<IActionResult> ToggleFeatured(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
            return NotFound();

        eventItem.IsFeatured = !eventItem.IsFeatured;
        eventItem.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isFeatured = eventItem.IsFeatured });
    }

    public class EventInput
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public bool IsFeatured { get; set; }
        public int? SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}