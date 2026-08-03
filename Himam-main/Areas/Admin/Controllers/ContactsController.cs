using Himam_main.Authorization;
using Himam_main.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManageContacts)]
[Route("Admin/Api/[controller]")]
public class ContactsController : Controller
{
    private readonly HimanAlhayahContext _context;

    public ContactsController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status = null)
    {
        var query = _context.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                c.Phone,
                c.Message,
                c.Status,
                c.Notes,
                c.RepliedBy,
                c.CreatedAt
            })
            .Take(100)
            .ToListAsync();

        return Json(new { total = items.Count, items });
    }

    [HttpPost("{id:int}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? notes)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact is null)
            return NotFound();

        contact.Status = status;
        if (!string.IsNullOrWhiteSpace(notes))
            contact.Notes = notes;
        contact.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }
}
