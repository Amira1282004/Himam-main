using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ViewAuditLogs)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class AuditLogsController : Controller
{
    private readonly HimanAlhayahContext _context;

    public AuditLogsController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? operationType = null,
        string? entityType = null,
        bool? success = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .Where(a => !a.IsArchived)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(operationType))
            query = query.Where(a => a.OperationType == operationType);

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (success.HasValue)
            query = query.Where(a => a.Success == success.Value);

        if (startDate.HasValue)
            query = query.Where(a => a.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Action,
                a.OperationType,
                a.EntityType,
                a.EntityId,
                a.Details,
                a.UserName,
                a.UserId,
                a.Success,
                a.IpAddress,
                a.UserAgent,
                a.CreatedAt
            })
            .ToListAsync();

        return Json(new
        {
            total = totalCount,
            page,
            pageSize,
            items = logs
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var log = await _context.AuditLogs
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (log is null)
            return NotFound();

        return Json(new
        {
            log.Id,
            log.Action,
            log.OperationType,
            log.EntityType,
            log.EntityId,
            log.Details,
            log.ChangesBefore,
            log.ChangesAfter,
            log.UserName,
            log.UserId,
            log.Success,
            log.IpAddress,
            log.UserAgent,
            log.CreatedAt,
            log.IsArchived,
            log.ArchivedAt
        });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = new
        {
            totalLogs = await _context.AuditLogs.CountAsync(),
            activeLogs = await _context.AuditLogs.CountAsync(a => !a.IsArchived),
            archivedLogs = await _context.AuditLogs.CountAsync(a => a.IsArchived),
            successfulOperations = await _context.AuditLogs.CountAsync(a => a.Success),
            failedOperations = await _context.AuditLogs.CountAsync(a => !a.Success),
            byOperationType = await _context.AuditLogs
                .Where(a => !a.IsArchived)
                .GroupBy(a => a.OperationType)
                .Select(g => new { operation = g.Key, count = g.Count() })
                .ToListAsync(),
            byEntityType = await _context.AuditLogs
                .Where(a => !a.IsArchived && a.EntityType != null)
                .GroupBy(a => a.EntityType)
                .Select(g => new { entity = g.Key, count = g.Count() })
                .ToListAsync(),
            recentActivity = await _context.AuditLogs
                .Where(a => !a.IsArchived)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .Select(a => new
                {
                    a.Action,
                    a.UserName,
                    a.OperationType,
                    a.EntityType,
                    a.Success,
                    a.CreatedAt
                })
                .ToListAsync()
        };

        return Json(stats);
    }

    [HttpGet("export")]
    [Authorize(Policy = AppPolicies.ManageSettings)]
    public async Task<IActionResult> Export(
        string? operationType = null,
        string? entityType = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(operationType))
            query = query.Where(a => a.OperationType == operationType);

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (startDate.HasValue)
            query = query.Where(a => a.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.CreatedAt <= endDate.Value);

        var logs = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Action,
                a.OperationType,
                a.EntityType,
                a.EntityId,
                a.Details,
                a.UserName,
                a.Success,
                a.IpAddress,
                a.UserAgent,
                a.CreatedAt
            })
            .ToListAsync();

        return Json(new
        {
            exportDate = DateTime.Now,
            totalRecords = logs.Count,
            records = logs
        });
    }
}
