using Himam_main.Data;
using Himam_main.Models;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public static class DatabaseSeeder
{
    private static readonly string[] RequiredRoles =
    [
        "Super Admin",
        "Site Manager",
        "Content Editor",
        "Customer Service"
    ];

    private const string SuperAdminRoleName = "Super Admin";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HimanAlhayahContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await EnsureSchemaPatchesAsync(context);
        await SeedRolesAsync(context);
        await MigrateLegacyAdminRoleAsync(context);
        await SeedSuperAdminAsync(context, passwordService, configuration);
        await SeedTeamMemberAsync(context, configuration);
        await SeedDefaultSettingsAsync(context);
        await SeedDefaultPagesAsync(context);
        await SeedServiceCategoriesAsync(context);
        await SeedSampleNewsAsync(context);
    }

    private static async Task EnsureSchemaPatchesAsync(HimanAlhayahContext context)
    {
        const string sql = """
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'FullName'
            )
            ALTER TABLE Users ADD FullName NVARCHAR(150) NULL;

            IF EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'AuditLogs' AND COLUMN_NAME = 'UserId' AND IS_NULLABLE = 'NO'
            )
            ALTER TABLE AuditLogs ALTER COLUMN UserId INT NULL;

            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'IsEmailVerified'
            )
            ALTER TABLE Users ADD IsEmailVerified BIT NOT NULL CONSTRAINT DF_Users_IsEmailVerified DEFAULT 1;

            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'VerificationCode'
            )
            ALTER TABLE Users ADD VerificationCode NVARCHAR(6) NULL;

            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'VerificationCodeExpires'
            )
            ALTER TABLE Users ADD VerificationCodeExpires DATETIME NULL;

            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = 'UserInvitations'
            )
            CREATE TABLE UserInvitations (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Email NVARCHAR(150) NOT NULL,
                FullName NVARCHAR(150) NULL,
                RoleName NVARCHAR(50) NOT NULL,
                Token NVARCHAR(64) NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                UsedAt DATETIME NULL,
                CreatedByUserId INT NULL,
                CreatedAt DATETIME NOT NULL CONSTRAINT DF_UserInvitations_CreatedAt DEFAULT GETDATE(),
                CONSTRAINT UQ_UserInvitations_Token UNIQUE (Token),
                CONSTRAINT FK_UserInvitations_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id) ON DELETE SET NULL
            );
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
    }

    private static async Task SeedRolesAsync(HimanAlhayahContext context)
    {
        var existingNames = await context.Roles
            .Select(r => r.Name)
            .ToListAsync();

        foreach (var roleName in RequiredRoles)
        {
            if (existingNames.Contains(roleName))
                continue;

            context.Roles.Add(new Role
            {
                Name = roleName,
                CreatedAt = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task MigrateLegacyAdminRoleAsync(HimanAlhayahContext context)
    {
        var legacyAdminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        if (legacyAdminRole is null)
            return;

        var superAdminRole = await context.Roles
            .FirstAsync(r => r.Name == SuperAdminRoleName);

        var linkedUsers = await context.Users
            .Include(u => u.Roles)
            .Where(u => u.Roles.Any(r => r.Id == legacyAdminRole.Id))
            .ToListAsync();

        foreach (var user in linkedUsers)
        {
            user.Roles.Remove(legacyAdminRole);

            if (!user.Roles.Any(r => r.Id == superAdminRole.Id))
                user.Roles.Add(superAdminRole);
        }

        context.Roles.Remove(legacyAdminRole);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(
        HimanAlhayahContext context,
        IPasswordService passwordService,
        IConfiguration configuration)
    {
        var username = configuration["SiteSettings:SuperAdmin:Username"] ?? "Admin_Z07";
        var email = configuration["SiteSettings:SuperAdmin:Email"] ?? "himamalhayah0@gmail.com";
        var fullName = configuration["SiteSettings:SuperAdmin:FullName"] ?? "ظافر الشهراني";
        var password = configuration["SiteSettings:SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(password))
            return;

        var superAdminRole = await context.Roles
            .FirstAsync(r => r.Name == SuperAdminRoleName);

        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == username || u.Username == "admin");

        if (user is null)
        {
            user = new User
            {
                Username = username,
                FullName = fullName,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            user.PasswordHash = passwordService.HashPassword(user, password);
            user.IsEmailVerified = true;
            user.Roles.Add(superAdminRole);
            context.Users.Add(user);
        }
        else
        {
            user.Username = username;
            user.FullName = fullName;
            user.Email = email;
            user.PasswordHash = passwordService.HashPassword(user, password);
            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.Now;

            if (!user.Roles.Any(r => r.Id == superAdminRole.Id))
                user.Roles.Add(superAdminRole);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedTeamMemberAsync(HimanAlhayahContext context, IConfiguration configuration)
    {
        var fullName = configuration["SiteSettings:SuperAdmin:FullName"] ?? "ظافر الشهراني";

        var exists = await context.TeamMembers
            .AnyAsync(t => t.Name == fullName);

        if (exists)
            return;

        context.TeamMembers.Add(new TeamMember
        {
            Name = fullName,
            Position = "المدير الأعلى · Super Admin",
            Bio = "مدير أعلى لمنصة همم الحياة — إدارة النظام والصلاحيات والفريق.",
            SortOrder = 1,
            CreatedAt = DateTime.Now
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedDefaultSettingsAsync(HimanAlhayahContext context)
    {
        var defaults = new Dictionary<string, (string Value, string Group)>
        {
            ["audit_log_retention_days"] = ("365", "Security"),
            ["site_name_ar"] = ("همم الحياة", "General"),
            ["site_name_en"] = ("Himam Alhayah", "General"),
            ["maintenance_mode"] = ("false", "General"),
            ["contact_email"] = ("info@himamalhayah.sa", "General"),
            ["contact_phone"] = ("0535105327", "General"),
            ["site_tagline"] = ("من الفكرة إلى التجربة", "General"),
            ["site_address"] = ("جدة، حي مشرفة، شارع عين الوهيط، مبنى 3654", "General"),
            ["contact_email_notify"] = ("false", "General")
        };

        var existingKeys = await context.Settings
            .Select(s => s.KeyName)
            .ToListAsync();

        foreach (var (key, (value, group)) in defaults)
        {
            if (existingKeys.Contains(key))
                continue;

            context.Settings.Add(new Setting
            {
                KeyName = key,
                Value = value,
                GroupName = group,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedDefaultPagesAsync(HimanAlhayahContext context)
    {
        var pages = new Dictionary<string, (string Title, string ContentAr, string MetaTitle, string MetaDesc)>
        {
            ["home"] = (
                "الرئيسية",
                """{"heroTitle":"نصنع تجارب ترفيهية... لأن الحياة حق للجميع","whatWeDo":"تنطلق همم الحياة من همّة تسعى إلى تحويل الأفكار والرسائل والأهداف إلى تجارب حقيقية ذات أثر ملموس."}""",
                "همم الحياة | تجارب وفعاليات",
                "همم الحياة — شركة سعودية متخصصة في تصميم وتنفيذ التجارب والفعاليات."
            ),
            ["about"] = (
                "من نحن",
                """{"heroTitle":"نُوحّد التفكير الاستراتيجي\nوالتنفيذ الدقيق في رؤية واحدة","heroLede":"همم الحياة الشركة المتخصصة التابعة لمجموعة الشهراني للأعمال في تصميم وتطوير وتنفيذ التجارب الترفيهية والفعاليات والمشروعات المتكاملة.","story":"تنطلق همم الحياة من همّة تسعى إلى تحويل الأفكار والرسائل والأهداف إلى تجارب حقيقية ذات أثر ملموس.","vision":"أن نرسّخ مكانتنا كشركة سعودية رائدة في صناعة التجارب والفعاليات.","mission":"تقديم حلول متكاملة تجمع بين الاستراتيجية والإبداع والتنفيذ المنضبط."}""",
                "من نحن | همم الحياة",
                "همم الحياة شركة سعودية تجمع بين التفكير الاستراتيجي والتصميم الإبداعي."
            ),
            ["contact"] = (
                "تواصل معنا",
                """{"heroTitle":"لنحوّل هدفك القادم\nإلى تجربة","heroLede":"شاركنا تفاصيل مشروعك، وسيتواصل معك فريقنا خلال يومي عمل."}""",
                "تواصل معنا | همم الحياة",
                "تواصل مع فريق همم الحياة."
            ),
            ["news"] = (
                "أخبار وفعاليات",
                "{}",
                "أخبار وفعاليات | همم الحياة",
                "آخر أخبار وفعاليات همم الحياة."
            )
        };

        var existing = await context.Pages.Select(p => p.Slug).ToListAsync();
        foreach (var (slug, (title, content, metaTitle, metaDesc)) in pages)
        {
            if (existing.Contains(slug))
                continue;

            context.Pages.Add(new Page
            {
                Slug = slug,
                Title = title,
                ContentAr = content,
                MetaTitle = metaTitle,
                MetaDescription = metaDesc,
                Status = "published",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedServiceCategoriesAsync(HimanAlhayahContext context)
    {
        if (await context.ServiceCategories.AnyAsync())
            return;

        var services = new[]
        {
            ("القطاع الحكومي", "حلول وفعاليات للجهات الحكومية.", 1),
            ("القطاع الخاص", "تجارب مؤسسية للشركات والعلامات.", 2),
            ("القطاع غير الربحي", "برامج وفعاليات ذات أثر مجتمعي.", 3),
            ("الترفيه والثقافة", "تصميم وتنفيذ تجارب ترفيهية.", 4),
            ("السياحة والضيافة", "فعاليات وتجارب سياحية.", 5),
            ("الرياضة", "فعاليات وبطولات رياضية.", 6)
        };

        foreach (var (title, desc, order) in services)
        {
            context.ServiceCategories.Add(new ServiceCategory
            {
                Title = title,
                DescriptionAr = desc,
                SortOrder = order,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedSampleNewsAsync(HimanAlhayahContext context)
    {
        if (await context.News.AnyAsync())
            return;

        var samples = new[]
        {
            new News
            {
                Title = "إطلاق موسم الفعاليات الترفيهية بجدة",
                Slug = "jeddah-entertainment-season-2026",
                ContentAr = "<p>أعلنت همم الحياة عن انطلاق موسمها الجديد من الفعاليات الترفيهية في مدينة جدة.</p>",
                Status = "published",
                MetaTitle = "إطلاق موسم الفعاليات الترفيهية بجدة",
                MetaDescription = "همم الحياة تدشّن موسماً جديداً من التجارب الترفيهية في جدة.",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-10)
            },
            new News
            {
                Title = "همم الحياة شريك رسمي لبرنامج تنمية القدرات",
                Slug = "capacity-building-partnership",
                ContentAr = "<p>أعلنت همم الحياة عن شراكتها الرسمية مع برنامج تنمية القدرات.</p>",
                Status = "published",
                MetaTitle = "شراكة تنمية القدرات",
                MetaDescription = "همم الحياة شريك رسمي لبرنامج تنمية القدرات.",
                CreatedAt = DateTime.Now.AddDays(-20),
                UpdatedAt = DateTime.Now.AddDays(-20)
            },
            new News
            {
                Title = "توقيع شراكة استراتيجية مع قطاع السياحة",
                Slug = "tourism-strategic-partnership",
                ContentAr = "<p>وقّعت همم الحياة شراكة استراتيجية مع قطاع السياحة.</p>",
                Status = "published",
                MetaTitle = "شراكة قطاع السياحة",
                MetaDescription = "توقيع شراكة استراتيجية مع قطاع السياحة.",
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now.AddDays(-30)
            }
        };

        context.News.AddRange(samples);
        await context.SaveChangesAsync();
    }
}
