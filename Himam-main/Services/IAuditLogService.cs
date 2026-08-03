namespace Himam_main.Services;

public interface IAuditLogService
{
    Task LogAsync(
        string action,
        int? userId,
        bool success,
        string? details = null,
        object? changes = null,
        HttpContext? httpContext = null);

    Task LogEntityChangeAsync(
        string entityName,
        string actionType,
        int? userId,
        int? entityId,
        object? oldValues = null,
        object? newValues = null,
        HttpContext? httpContext = null);

    Task<int> PurgeOlderThanAsync(int retentionDays);
}
