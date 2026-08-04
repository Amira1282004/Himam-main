using Himam_main.Data;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public class AuditLogArchiverService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditLogArchiverService> _logger;

    // Retention period: 2 years (730 days)
    // After 2 years, logs are archived and can still be viewed but marked as archived
    // After 5 years (1825 days), archived logs can be permanently deleted if needed
    private const int ArchiveRetentionDays = 730; // 2 years
    private const int DeleteRetentionDays = 1825; // 5 years

    public AuditLogArchiverService(
        IServiceProvider serviceProvider,
        ILogger<AuditLogArchiverService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit Log Archiver Service started.");

        // Run immediately on startup
        await ArchiveOldLogsAsync(stoppingToken);

        // Run daily at 2:00 AM
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
            
            if (now > nextRun)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            _logger.LogInformation($"Next archive run scheduled for: {nextRun}");

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await ArchiveOldLogsAsync(stoppingToken);
            }
        }

        _logger.LogInformation("Audit Log Archiver Service stopped.");
    }

    private async Task ArchiveOldLogsAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting audit log archival process...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HimanAlhayahContext>();

            // Archive logs older than 2 years
            var archiveCutoff = DateTime.Now.AddDays(-ArchiveRetentionDays);
            var logsToArchive = await context.AuditLogs
                .Where(a => a.CreatedAt < archiveCutoff && !a.IsArchived)
                .CountAsync(stoppingToken);

            if (logsToArchive > 0)
            {
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE AuditLogs SET IsArchived = 1, ArchivedAt = GETDATE() WHERE CreatedAt < {0} AND IsArchived = 0",
                    archiveCutoff,
                    stoppingToken);

                _logger.LogInformation($"Archived {logsToArchive} audit logs older than {ArchiveRetentionDays} days.");
            }

            // Optionally delete logs older than 5 years (commented out by default for safety)
            // Uncomment this section if you want to permanently delete very old logs
            /*
            var deleteCutoff = DateTime.Now.AddDays(-DeleteRetentionDays);
            var logsToDelete = await context.AuditLogs
                .Where(a => a.CreatedAt < deleteCutoff && a.IsArchived)
                .CountAsync(stoppingToken);

            if (logsToDelete > 0)
            {
                await context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM AuditLogs WHERE CreatedAt < {0} AND IsArchived = 1",
                    deleteCutoff,
                    stoppingToken);

                _logger.LogInformation($"Deleted {logsToDelete} archived audit logs older than {DeleteRetentionDays} days.");
            }
            */

            _logger.LogInformation("Audit log archival process completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during audit log archival process.");
        }
    }
}
