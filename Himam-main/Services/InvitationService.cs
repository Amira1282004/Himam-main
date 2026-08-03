using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public class InvitationService : IInvitationService
{
    private static readonly Dictionary<string, string> RoleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["manager"] = AppRoles.SiteManager,
        ["editor"] = AppRoles.ContentEditor,
        ["support"] = AppRoles.CustomerService,
        ["site manager"] = AppRoles.SiteManager,
        ["content editor"] = AppRoles.ContentEditor,
        ["customer service"] = AppRoles.CustomerService
    };

    private readonly HimanAlhayahContext _context;
    private readonly IEmailService _email;
    private readonly IPasswordService _passwordService;
    private readonly IAuditLogService _auditLog;

    public InvitationService(
        HimanAlhayahContext context,
        IEmailService email,
        IPasswordService passwordService,
        IAuditLogService auditLog)
    {
        _context = context;
        _email = email;
        _passwordService = passwordService;
        _auditLog = auditLog;
    }

    public string MapRoleKey(string roleKey)
    {
        if (RoleMap.TryGetValue(roleKey.Trim(), out var role))
            return role;
        throw new ArgumentException("دور غير صالح.");
    }

    public async Task<UserInvitation> SendInvitationAsync(
        string email,
        string? fullName,
        string roleKey,
        int invitedByUserId,
        string baseUrl)
    {
        email = email.Trim().ToLowerInvariant();
        var roleName = MapRoleKey(roleKey);

        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("يوجد حساب مسجّل بهذا البريد.");

        var pending = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Email == email && i.UsedAt == null && i.ExpiresAt > DateTime.Now);

        if (pending is not null)
        {
            pending.FullName = fullName?.Trim();
            pending.RoleName = roleName;
            pending.ExpiresAt = DateTime.Now.AddDays(7);
            pending.Token = Guid.NewGuid().ToString("N");
        }
        else
        {
            pending = new UserInvitation
            {
                Email = email,
                FullName = fullName?.Trim(),
                RoleName = roleName,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.Now.AddDays(7),
                CreatedByUserId = invitedByUserId,
                CreatedAt = DateTime.Now
            };
            _context.UserInvitations.Add(pending);
        }

        await _context.SaveChangesAsync();

        var link = $"{baseUrl.TrimEnd('/')}/Admin/Account/AcceptInvite?token={pending.Token}";
        var roleAr = RoleArabicName(roleName);

        await _email.SendAsync(
            email,
            "دعوة للانضمام إلى لوحة تحكم همم الحياة",
            $"""
            <div dir="rtl" style="font-family:Tahoma,sans-serif;line-height:1.7;">
              <h2>مرحباً{(!string.IsNullOrWhiteSpace(fullName) ? $" {fullName}" : "")}</h2>
              <p>تمت دعوتك للانضمام إلى لوحة تحكم <strong>همم الحياة</strong> بصفة <strong>{roleAr}</strong>.</p>
              <p>اضغط الزر أدناه لإنشاء حسابك وإدخال بياناتك:</p>
              <p><a href="{link}" style="display:inline-block;padding:12px 24px;background:#0d9488;color:#fff;text-decoration:none;border-radius:8px;">إنشاء حسابي</a></p>
              <p style="color:#666;font-size:13px;">أو انسخ الرابط:<br>{link}</p>
              <p style="color:#666;font-size:13px;">تنتهي صلاحية الدعوة خلال 7 أيام.</p>
            </div>
            """);

        await _auditLog.LogAsync(
            "InvitationSent",
            invitedByUserId,
            success: true,
            details: $"دعوة إلى {email} ({roleName})");

        return pending;
    }

    public async Task<UserInvitation?> GetValidInvitationAsync(string token)
    {
        return await _context.UserInvitations
            .FirstOrDefaultAsync(i =>
                i.Token == token &&
                i.UsedAt == null &&
                i.ExpiresAt > DateTime.Now);
    }

    public async Task<User> RegisterFromInvitationAsync(
        string token,
        string fullName,
        string username,
        string password)
    {
        var invitation = await GetValidInvitationAsync(token)
            ?? throw new InvalidOperationException("رابط الدعوة غير صالح أو منتهٍ.");

        if (await _context.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("اسم المستخدم مستخدم مسبقاً.");

        if (await _context.Users.AnyAsync(u => u.Email == invitation.Email))
            throw new InvalidOperationException("يوجد حساب مسجّل بهذا البريد.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == invitation.RoleName)
            ?? throw new InvalidOperationException("الدور غير موجود.");

        var code = Random.Shared.Next(100000, 999999).ToString();
        var user = new User
        {
            Username = username.Trim(),
            FullName = fullName.Trim(),
            Email = invitation.Email,
            IsEmailVerified = false,
            VerificationCode = code,
            VerificationCodeExpires = DateTime.Now.AddMinutes(15),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        user.PasswordHash = _passwordService.HashPassword(user, password);
        user.Roles.Add(role);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SendVerificationEmailAsync(user, code);

        await _auditLog.LogAsync(
            "UserRegisteredFromInvite",
            user.Id,
            success: true,
            details: $"تسجيل من دعوة: {user.Email}");

        return user;
    }

    public async Task<bool> VerifyEmailCodeAsync(int userId, string code)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return false;

        if (user.IsEmailVerified)
            return true;

        if (user.VerificationCodeExpires < DateTime.Now)
            throw new InvalidOperationException("انتهت صلاحية رمز التأكيد. اطلب رمزاً جديداً.");

        if (!string.Equals(user.VerificationCode, code.Trim(), StringComparison.Ordinal))
            throw new InvalidOperationException("رمز التأكيد غير صحيح.");

        user.IsEmailVerified = true;
        user.VerificationCode = null;
        user.VerificationCodeExpires = null;
        user.UpdatedAt = DateTime.Now;

        var invitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Email == user.Email && i.UsedAt == null);
        if (invitation is not null)
            invitation.UsedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            "EmailVerified",
            userId,
            success: true,
            details: $"تفعيل البريد: {user.Email}");

        return true;
    }

    public async Task ResendVerificationCodeAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException("المستخدم غير موجود.");

        if (user.IsEmailVerified)
            throw new InvalidOperationException("البريد مُفعَّل مسبقاً.");

        var code = Random.Shared.Next(100000, 999999).ToString();
        user.VerificationCode = code;
        user.VerificationCodeExpires = DateTime.Now.AddMinutes(15);
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        await SendVerificationEmailAsync(user, code);
    }

    private async Task SendVerificationEmailAsync(User user, string code)
    {
        await _email.SendAsync(
            user.Email,
            "رمز تأكيد حسابك — همم الحياة",
            $"""
            <div dir="rtl" style="font-family:Tahoma,sans-serif;line-height:1.7;">
              <h2>تأكيد حسابك</h2>
              <p>مرحباً {user.FullName ?? user.Username}،</p>
              <p>رمز التأكيد المكوّن من 6 أرقام:</p>
              <p style="font-size:28px;font-weight:bold;letter-spacing:6px;color:#0d9488;">{code}</p>
              <p style="color:#666;font-size:13px;">صالح لمدة 15 دقيقة.</p>
            </div>
            """);
    }

    private static string RoleArabicName(string role) => role switch
    {
        AppRoles.SiteManager => "مدير الموقع",
        AppRoles.ContentEditor => "محرر المحتوى",
        AppRoles.CustomerService => "خدمة العملاء",
        AppRoles.SuperAdmin => "المدير الأعلى",
        _ => role
    };
}
