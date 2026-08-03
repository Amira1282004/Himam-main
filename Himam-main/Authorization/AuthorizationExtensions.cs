using Microsoft.AspNetCore.Authorization;

namespace Himam_main.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
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

            options.AddPolicy(AppPolicies.ManagePages, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageServices, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.ManageContacts, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.CustomerService));

            options.AddPolicy(AppPolicies.ViewReports, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.EditContent, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.ContentEditor));

            options.AddPolicy(AppPolicies.PublishContent, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager));

            options.AddPolicy(AppPolicies.UploadMedia, p =>
                p.RequireRole(AppRoles.SuperAdmin, AppRoles.SiteManager, AppRoles.ContentEditor));
        });

        return services;
    }
}
