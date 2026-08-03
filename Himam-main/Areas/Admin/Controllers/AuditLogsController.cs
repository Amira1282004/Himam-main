using Himam_main.Authorization;
using Himam_main.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ViewAuditLogs)]
[Route("Admin/Api/[controller]")]
public class AuditLogsController : Controller
{
    private readonly HimanAlhayahContext _context;

    public AuditLogsController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 10, 100);

        var query = _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Action,
                a.IpAddress,
                a.CreatedAt,
                UserName = a.User != null ? (a.User.FullName ?? a.User.Username) : "—",
                Details = ParseDetails(a.Details)
            })
            .ToListAsync();

        return Json(new { total, page, pageSize, items });
    }

    private static object? ParseDetails(string? details)
    {
        if (string.IsNullOrWhiteSpace(details))
            return null;

        try
        {
            return JsonSerializer.Deserialize<object>(details);
        }
        catch
        {
            return details;
        }
    }
}
