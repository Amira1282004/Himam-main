using Himam_main.Data;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HimanAlhayahContext _context;

    public RedirectMiddleware(RequestDelegate next, HimanAlhayahContext context)
    {
        _next = next;
        _context = context;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Check if this path has a redirect
        var redirect = await _context.UrlRedirects
            .FirstOrDefaultAsync(r => r.OldUrl == path && r.IsActive);

        if (redirect != null)
        {
            context.Response.Redirect(redirect.NewUrl, true); // Permanent redirect (301)
            return;
        }

        await _next(context);
    }
}
