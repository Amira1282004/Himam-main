using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Himam_main.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // ==================== SUPER ADMIN (Full Access) ====================
            options.AddPolicy(AppPolicies.ManageUsers, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            options.AddPolicy(AppPolicies.ManagePermissions, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            options.AddPolicy(AppPolicies.ManageSettings, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            options.AddPolicy(AppPolicies.ViewAuditLogs, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            options.AddPolicy(AppPolicies.ManageIntegrations, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            options.AddPolicy(AppPolicies.ManageAdmins, p =>
                p.RequireRole(AppRoles.SuperAdmin));

            // ==================== SITE MANAGER ====================
            options.AddPolicy(AppPolicies.ManagePages, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageServices, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageEvents, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageNews, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ViewReports, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageNotifications, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            // ==================== CONTENT EDITOR ====================
            options.AddPolicy(AppPolicies.EditContent, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.ContentEditor));

            options.AddPolicy(AppPolicies.UploadMedia, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.ContentEditor));

            // Note: Content Editors can edit but NOT publish (requires separate PublishContent policy)

            // ==================== PUBLISH CONTENT (Publishing requires explicit permission) ====================
            options.AddPolicy(AppPolicies.PublishContent, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            // ==================== CUSTOMER SERVICE ====================
            options.AddPolicy(AppPolicies.ManageContacts, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.CustomerService));
        });

        return services;
    }
}

public static class CurrentUserExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    public static bool CanPublish(this ClaimsPrincipal user)
    {
        return user.IsInRole(AppRoles.SuperAdmin) || user.IsInRole(AppRoles.SiteManager);
    }
}
