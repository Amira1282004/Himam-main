using Himam_main.Authorization;
using Himam_main.Services;
using System.Text.Json;

namespace Himam_main.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for static files and health checks
        if (context.Request.Path.StartsWithSegments("/wwwroot") || 
            context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/favicon.ico"))
        {
            await _next(context);
            return;
        }

        // Store original response body
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Log successful operations
            await LogOperationAsync(context, true);
        }
        catch (Exception ex)
        {
            // Log failed operations
            await LogOperationAsync(context, false, ex.Message);
            throw;
        }
        finally
        {
            // Restore original response body
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogOperationAsync(HttpContext context, bool success, string? errorMessage = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
        
        var userId = context.User?.GetUserId();
        var userName = context.User?.Identity?.Name;
        
        var operationType = DetermineOperationType(context);
        var entityType = DetermineEntityType(context);
        var entityId = ExtractEntityId(context);

        var details = $"{context.Request.Method} {context.Request.Path}";
        if (!string.IsNullOrWhiteSpace(errorMessage))
            details += $" - Error: {errorMessage}";

        // Log to audit service
        await auditLogService.LogAsync(
            action: operationType,
            userId: userId,
            success: success,
            details: details,
            httpContext: context
        );

        // For entity operations, log entity changes
        if (!string.IsNullOrWhiteSpace(entityType) && entityId.HasValue)
        {
            await auditLogService.LogEntityChangeAsync(
                entityName: entityType,
                actionType: operationType,
                userId: userId,
                entityId: entityId.Value,
                httpContext: context
            );
        }
    }

    private static string DetermineOperationType(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value?.ToLower();

        return method switch
        {
            "POST" => "Create",
            "PUT" => "Update",
            "PATCH" => "Update",
            "DELETE" => "Delete",
            "GET" => path?.Contains("login") == true ? "Login" : 
                     path?.Contains("logout") == true ? "Logout" : 
                     "View",
            _ => "General"
        };
    }

    private static string? DetermineEntityType(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path?.Contains("/news") == true) return "News";
        if (path?.Contains("/pages") == true) return "Page";
        if (path?.Contains("/events") == true) return "Event";
        if (path?.Contains("/services") == true) return "Service";
        if (path?.Contains("/users") == true) return "User";
        if (path?.Contains("/settings") == true) return "Setting";
        if (path?.Contains("/media") == true) return "Media";
        if (path?.Contains("/contacts") == true) return "Contact";
        if (path?.Contains("/seo") == true) return "SeoSetting";
        if (path?.Contains("/company") == true) return "CompanyInfo";

        return null;
    }

    private static int? ExtractEntityId(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Try to extract ID from URL pattern like /api/controller/{id}
        var segments = path?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments != null && segments.Length > 0)
        {
            var lastSegment = segments.Last();
            if (int.TryParse(lastSegment, out var id))
                return id;
        }

        return null;
    }
}
