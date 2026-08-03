namespace Himam_main.ViewModels;

public class AcceptInviteViewModel
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? SuggestedFullName { get; set; }
    public string RoleArabic { get; set; } = string.Empty;
    public string Step { get; set; } = "register";
    public int? UserId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
