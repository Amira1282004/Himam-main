namespace Himam_main.Authorization;

public static class AppRoles
{
    public const string SuperAdmin = "Super Admin";
    public const string SiteManager = "Site Manager";
    public const string ContentEditor = "Content Editor";
    public const string CustomerService = "Customer Service";

    public static readonly string[] All =
    [
        SuperAdmin,
        SiteManager,
        ContentEditor,
        CustomerService
    ];
}

public static class AppPolicies
{
    public const string ManageUsers = "ManageUsers";
    public const string ManagePermissions = "ManagePermissions";
    public const string ManageSettings = "ManageSettings";
    public const string ViewAuditLogs = "ViewAuditLogs";
    public const string ManageIntegrations = "ManageIntegrations";
    public const string ManageAdmins = "ManageAdmins";
    public const string ManagePages = "ManagePages";
    public const string ManageServices = "ManageServices";
    public const string ManageContacts = "ManageContacts";
    public const string ViewReports = "ViewReports";
    public const string EditContent = "EditContent";
    public const string PublishContent = "PublishContent";
    public const string UploadMedia = "UploadMedia";
}
