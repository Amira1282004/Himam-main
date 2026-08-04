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
    // User Management
    public const string ManageUsers = "ManageUsers";
    public const string ManagePermissions = "ManagePermissions";
    public const string ManageAdmins = "ManageAdmins";
    
    // System Settings
    public const string ManageSettings = "ManageSettings";
    public const string ManageIntegrations = "ManageIntegrations";
    
    // Security & Audit
    public const string ViewAuditLogs = "ViewAuditLogs";
    
    // Content Management
    public const string ManagePages = "ManagePages";
    public const string ManageServices = "ManageServices";
    public const string ManageEvents = "ManageEvents";
    public const string ManageNews = "ManageNews";
    public const string EditContent = "EditContent";
    public const string PublishContent = "PublishContent";
    public const string UploadMedia = "UploadMedia";
    
    // Customer Service
    public const string ManageContacts = "ManageContacts";
    public const string ViewReports = "ViewReports";
    
    // Notifications
    public const string ManageNotifications = "ManageNotifications";
}
