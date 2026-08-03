using Himam_main.Data;
using Himam_main.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public interface IDashboardProfileService
{
    Task<DashboardProfileViewModel?> GetProfileAsync(int userId);
}

public class DashboardProfileService : IDashboardProfileService
{
    private static readonly Dictionary<string, string> RoleArabicNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Super Admin"] = "المدير الأعلى",
        ["Site Manager"] = "مدير الموقع",
        ["Content Editor"] = "محرر المحتوى",
        ["Customer Service"] = "خدمة العملاء"
    };

    private readonly HimanAlhayahContext _context;

    public DashboardProfileService(HimanAlhayahContext context)
    {
        _context = context;
    }

    public async Task<DashboardProfileViewModel?> GetProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return null;

        var roles = user.Roles.Select(r => r.Name).OrderBy(n => n).ToList();
        var primaryRole = roles.FirstOrDefault(r => r.Equals("Super Admin", StringComparison.OrdinalIgnoreCase))
            ?? roles.FirstOrDefault()
            ?? "User";

        return new DashboardProfileViewModel
        {
            UserId = user.Id,
            FullName = user.FullName ?? user.Username,
            Username = user.Username,
            Email = user.Email,
            PrimaryRole = primaryRole,
            PrimaryRoleArabic = RoleArabicNames.GetValueOrDefault(primaryRole, primaryRole),
            Initials = BuildInitials(user.FullName ?? user.Username),
            Roles = roles
        };
    }

    private static string BuildInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();

        return name.Length >= 2 ? name[..2].ToUpperInvariant() : name.ToUpperInvariant();
    }
}
