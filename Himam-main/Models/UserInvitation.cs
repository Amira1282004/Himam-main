namespace Himam_main.Models;

public partial class UserInvitation
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string RoleName { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }
}
