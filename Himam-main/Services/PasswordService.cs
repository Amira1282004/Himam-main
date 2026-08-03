using Himam_main.Models;
using Microsoft.AspNetCore.Identity;

namespace Himam_main.Services;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password, string passwordHash);
}

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password) =>
        _hasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string password, string passwordHash) =>
        _hasher.VerifyHashedPassword(user, passwordHash, password) != PasswordVerificationResult.Failed;
}
