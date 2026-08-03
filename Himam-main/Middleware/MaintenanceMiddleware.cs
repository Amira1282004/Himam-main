using Himam_main.Data;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Middleware;

public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;

    public MaintenanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, HimanAlhayahContext db)
    {
        var path = context.Request.Path.Value ?? "";

        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var maintenance = await db.Settings
            .AsNoTracking()
            .Where(s => s.KeyName == "maintenance_mode")
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (maintenance == "true" && context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = 503;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync("""
                <!DOCTYPE html><html lang="ar" dir="rtl"><head><meta charset="utf-8"><title>صيانة | همم الحياة</title></head>
                <body style="font-family:sans-serif;text-align:center;padding:80px 20px;">
                <h1>الموقع قيد الصيانة</h1>
                <p>نعمل على تحسين تجربتكم. نعود قريباً.</p>
                </body></html>
                """);
            return;
        }

        await _next(context);
    }
}
