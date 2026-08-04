namespace Himam_main.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, RateLimitEntry> _rateLimitStore;
    private readonly TimeSpan _slidingWindow;
    private readonly int _maxRequests;

    public RateLimitMiddleware(RequestDelegate next, int maxRequests = 100, int windowInSeconds = 60)
    {
        _next = next;
        _maxRequests = maxRequests;
        _slidingWindow = TimeSpan.FromSeconds(windowInSeconds);
        _rateLimitStore = new Dictionary<string, RateLimitEntry>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientId(context);
        
        // Clean up old entries periodically
        CleanupOldEntries();

        if (_rateLimitStore.TryGetValue(clientId, out var entry))
        {
            // Check if within window
            if (DateTime.UtcNow - entry.WindowStart < _slidingWindow)
            {
                if (entry.RequestCount >= _maxRequests)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }
                entry.RequestCount++;
            }
            else
            {
                // Reset window
                entry.WindowStart = DateTime.UtcNow;
                entry.RequestCount = 1;
            }
        }
        else
        {
            _rateLimitStore[clientId] = new RateLimitEntry
            {
                WindowStart = DateTime.UtcNow,
                RequestCount = 1
            };
        }

        await _next(context);
    }

    private string GetClientId(HttpContext context)
    {
        // Use IP address as client identifier
        var ip = context.Connection.RemoteIpAddress?.ToString();
        return ip ?? "unknown";
    }

    private void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow - _slidingWindow;
        var keysToRemove = _rateLimitStore
            .Where(kvp => kvp.Value.WindowStart < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _rateLimitStore.Remove(key);
        }
    }

    private class RateLimitEntry
    {
        public DateTime WindowStart { get; set; }
        public int RequestCount { get; set; }
    }
}
