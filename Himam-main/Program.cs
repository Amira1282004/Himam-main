using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Middleware;
using Himam_main.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HimanAlhayahContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        
        // Security settings for cookies - Secure for HTTPS only
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always require HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddControllersWithViews(options =>
{
    // Auto-validate AntiForgeryToken for all POST requests
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register Audit Log Service
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Register Security Services
builder.Services.AddScoped<FileUploadSecurityService>();
builder.Services.AddScoped<BruteForceProtectionService>();

// Register Background Services
builder.Services.AddHostedService<AuditLogArchiverService>();

// Register Authorization
builder.Services.AddAppAuthorization();

var app = builder.Build();

// Use Security Headers Middleware (first)
app.UseMiddleware<SecurityHeadersMiddleware>();

// Use Rate Limiting Middleware
app.UseMiddleware<RateLimitMiddleware>();

// Use HTTPS Redirection (force HTTP to HTTPS)
app.UseHttpsRedirection();

// Use Static Files
app.UseStaticFiles();

// Use Routing
app.UseRouting();

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Use Audit Middleware (after routing, before endpoints)
app.UseMiddleware<AuditMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HimanAlhayahContext>();
    await DbSeeder.SeedAsync(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/User/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "User" })
    .WithStaticAssets();

app.Run();
