namespace Himam_main.Models.ViewModels;

public class DashboardViewModel
{
    public CurrentUserViewModel CurrentUser { get; set; } = null!;
    public List<UserListItemViewModel> Users { get; set; } = [];
    public int TotalUsers { get; set; }
    public Dictionary<string, int> UsersByRole { get; set; } = [];
    public string? SuccessMessage { get; set; }
}

public class CurrentUserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string RoleNameAr { get; set; } = null!;
    public string RoleBadgeClass { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}

public class UserListItemViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string RoleNameAr { get; set; } = null!;
    public string RoleBadgeClass { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public bool IsCurrentUser { get; set; }
}

public class ProfileEditViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
