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

        var payload = new Dictionary<string, object?>
        {
            ["result"] = success ? "success" : "failure",
            ["userAgent"] = httpContext?.Request.Headers.UserAgent.ToString(),
            ["details"] = details
        };

        if (changes is not null)
            payload["changes"] = changes;

        var entry = new AuditLog
        {
            Action = action,
            UserId = userId ?? 0,
            Details = JsonSerializer.Serialize(payload),
            IpAddress = GetClientIp(httpContext),
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
