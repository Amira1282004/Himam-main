using System.Text.Json;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public class AuditLogService : IAuditLogService
{
    private readonly HimanAlhayahContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(HimanAlhayahContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string action,
        int? userId,
        bool success,
        string? details = null,
        object? changes = null,
        HttpContext? httpContext = null)
    {
        httpContext ??= _httpContextAccessor.HttpContext;

        var entry = new AuditLog
        {
            Action = action,
            OperationType = ExtractOperationType(action),
            UserId = userId,
            UserName = await GetUserNameAsync(userId),
            Details = details,
            ChangesAfter = changes is not null ? JsonSerializer.Serialize(changes) : null,
            Success = success,
            IpAddress = GetClientIp(httpContext),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.Now
        };

        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task LogEntityChangeAsync(
        string entityName,
        string actionType, // Create, Update, Delete
        int? userId,
        int? entityId,
        object? oldValues = null,
        object? newValues = null,
        HttpContext? httpContext = null)
    {
        httpContext ??= _httpContextAccessor.HttpContext;

        var entry = new AuditLog
        {
            Action = $"{entityName} - {actionType}",
            OperationType = actionType,
            EntityType = entityName,
            EntityId = entityId,
            UserId = userId,
            UserName = await GetUserNameAsync(userId),
            Details = $"{actionType} {entityName}",
            ChangesBefore = oldValues is not null ? JsonSerializer.Serialize(oldValues) : null,
            ChangesAfter = newValues is not null ? JsonSerializer.Serialize(newValues) : null,
            Success = true,
            IpAddress = GetClientIp(httpContext),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.Now
        };

        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<int> PurgeOlderThanAsync(int retentionDays)
    {
        var cutoff = DateTime.Now.AddDays(-retentionDays);
        return await _context.AuditLogs
            .Where(a => a.CreatedAt < cutoff)
            .ExecuteDeleteAsync();
    }

    public async Task<int> ArchiveOlderThanAsync(int retentionDays)
    {
        var cutoff = DateTime.Now.AddDays(-retentionDays);
        var logsToArchive = await _context.AuditLogs
            .Where(a => a.CreatedAt < cutoff && !a.IsArchived)
            .ToListAsync();

        foreach (var log in logsToArchive)
        {
            log.IsArchived = true;
            log.ArchivedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return logsToArchive.Count;
    }

    private static string? ExtractOperationType(string action)
    {
        if (action.Contains("Create", StringComparison.OrdinalIgnoreCase))
            return "Create";
        if (action.Contains("Update", StringComparison.OrdinalIgnoreCase))
            return "Update";
        if (action.Contains("Delete", StringComparison.OrdinalIgnoreCase))
            return "Delete";
        if (action.Contains("Login", StringComparison.OrdinalIgnoreCase))
            return "Login";
        if (action.Contains("Logout", StringComparison.OrdinalIgnoreCase))
            return "Logout";
        if (action.Contains("Publish", StringComparison.OrdinalIgnoreCase))
            return "Publish";
        return "General";
    }

    private async Task<string?> GetUserNameAsync(int? userId)
    {
        if (!userId.HasValue || userId.Value == 0)
            return null;

        var user = await _context.Users.FindAsync(userId.Value);
        return user?.Username;
    }

    private static string? GetClientIp(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
            return forwarded.Split(',')[0].Trim();

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
