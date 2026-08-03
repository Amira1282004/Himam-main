namespace Himam_main.ViewModels;

public class DashboardProfileViewModel
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PrimaryRole { get; set; } = string.Empty;

    public string PrimaryRoleArabic { get; set; } = string.Empty;

    public string Initials { get; set; } = string.Empty;

    public IReadOnlyList<string> Roles { get; set; } = [];
}
