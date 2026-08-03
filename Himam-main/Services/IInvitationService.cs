using Himam_main.Models;

namespace Himam_main.Services;

public interface IInvitationService
{
    Task<UserInvitation> SendInvitationAsync(string email, string? fullName, string roleKey, int invitedByUserId, string baseUrl);
    Task<UserInvitation?> GetValidInvitationAsync(string token);
    Task<User> RegisterFromInvitationAsync(string token, string fullName, string username, string password);
    Task<bool> VerifyEmailCodeAsync(int userId, string code);
    Task ResendVerificationCodeAsync(int userId);
    string MapRoleKey(string roleKey);
}
