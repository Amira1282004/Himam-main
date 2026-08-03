using Himam_main.Models;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Data;

public static class DbSeeder
{
    private static readonly string[] RoleNames =
    [
        "Super Admin",
        "Site Manager",
        "Content Editor",
        "Customer Service"
    ];

    public static async Task SeedAsync(HimanAlhayahContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            foreach (var roleName in RoleNames)
            {
                context.Roles.Add(new Role
                {
                    Name = roleName,
                    CreatedAt = DateTime.Now
                });
            }

            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var superAdminRole = await context.Roles
                .FirstAsync(r => r.Name == "Super Admin");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("F?ldd1880@");

            await context.Database.ExecuteSqlRawAsync(@"
                SET IDENTITY_INSERT Users ON;
                INSERT INTO Users (Id, Username, Email, PasswordHash, CreatedAt, UpdatedAt)
                VALUES (1, {0}, {1}, {2}, GETDATE(), GETDATE());
                SET IDENTITY_INSERT Users OFF;",
                "ظافر الشهراني", "seiframadan125@gmail.com", passwordHash);

            var user = await context.Users
                .Include(u => u.Roles)
                .FirstAsync(u => u.Id == 1);

            user.Roles.Add(superAdminRole);
            await context.SaveChangesAsync();
        }

        if (!await context.TeamMembers.AnyAsync())
        {
            var superAdminUser = await context.Users
                .FirstAsync(u => u.Id == 1);

            context.TeamMembers.Add(new TeamMember
            {
                Name = "ظافر الشهراني",
                Position = "المدير العام",
                Bio = "مدير عام ومؤسس منصة همم الحياة، يشرف على إدارة الموقع والفريق.",
                SortOrder = 1,
                CreatedAt = DateTime.Now,
                UserId = superAdminUser.Id
            });

            await context.SaveChangesAsync();
        }

        // Seed Hero Section
        if (!await context.HeroSections.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            context.HeroSections.Add(new HeroSection
            {
                Title = "نصنع تجارب ترفيهية لأن جودة الحياة حق للجميع",
                Subtitle = "صناعة الفعاليات وتطوير التجارب",
                CtaText = "استكشف خدماتنا",
                CtaLink = "#sectors",
                YoutubeVideoId = "c8bBHCEI9AE",
                IsVideoEnabled = true,
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = user.Id
            });
            await context.SaveChangesAsync();
        }

        // Seed About Section
        if (!await context.AboutSections.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            context.AboutSections.Add(new AboutSection
            {
                Eyebrow = "ما نقوم به",
                Title = "تنطلق همم الحياة من همة تسعى إلى تحويل الأفكار والرسائل والأهداف إلى تجارب حقيقية ذات أثر ملموس",
                Description = "نجمع بين التفكير الاستراتيجي والإبداع والتنفيذ المنضبط؛ لنقدم تجارب مترابطة تضع الإنسان في جوهرها، وتنتقل بالفكرة من التصور إلى واقع يُعاش ويُتذكر.",
                AdditionalDescription = "ولا تنتهي قيمة التجربة بانتهاء تنفيذها، بل تمتد إلى ما تحققه من نتائج، وما تتركه من أثر يمكن قياسه وتطويره والبناء عليه.",
                ChairmanName = "ظافر الشهراني",
                ChairmanTitle = "رئيس مجلس الإدارة",
                ChairmanImage = "~/assets/chairman-dhafer-alshahrani.png",
                IsVisible = true,
                SortOrder = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = user.Id
            });
            await context.SaveChangesAsync();
        }

        // Seed Sectors
        if (!await context.Sectors.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            var sectors = new[]
            {
                new Sector
                {
                    Title = "الجهات الحكومية والهيئات",
                    Description = "نطور المبادرات والفعاليات والبرامج والمشروعات التي تدعم الأهداف المؤسسية والمجتمعية وتخدم المستفيدين ضمن تجربة واضحة ومنظمة تراعي الحوكمة وتنوع الجمهور وقابلية قياس النتائج.",
                    Image = "~/assets/sectors/s1.JPG",
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "القطاع الخاص والعلامات التجارية",
                    Description = "نصمم تجارب تساعد الشركات والعلامات التجارية على ترجمة أهدافها ورسائلها إلى تفاعل حقيقي مع العملاء والموظفين والشركاء.",
                    Image = "~/assets/sectors/s2.jpg",
                    IsVisible = true,
                    SortOrder = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "القطاع غير الربحي",
                    Description = "نساعد القطاعات غير الربحية على تحويل رسالتها وأهدافها المجتمعية إلى برامج ومبادرات وتجارب تصل للمستفيدين والداعمين والمتطوعين بوضوح، وتدعم المشاركة والاستفادة ضمن الموارد المتاحة.",
                    Image = "~/assets/sectors/s3.jpg",
                    IsVisible = true,
                    SortOrder = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "القطاع الترفيهي والثقافي",
                    Description = "نطور فعاليات وتجارب تجمع بين المحتوى والإبداع والتفاعل، وتقدم للجمهور رحلة متماسكة تتناسب مع طبيعة المشروع وسياقه الثقافي والترفيهي.",
                    Image = "~/assets/sectors/s4.jpeg",
                    IsVisible = true,
                    SortOrder = 4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "القطاع السياحي والضيافة",
                    Description = "نصمم تجارب الزوار والوجهات والبرامج السياحية بما يعزز وضوح الرحلة وجودة التفاعل، ويربط المكان بقصته وهويته وجمهوره المستهدف.",
                    Image = "~/assets/sectors/s5.jpg",
                    IsVisible = true,
                    SortOrder = 5,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "القطاع الرياضي",
                    Description = "نطور فعاليات رياضية وتجارب تفاعلية تجمع بين الحماس والتنظيم، وتقدم للجمهور تجارب رياضية نوعية تناسب السياق السعودي.",
                    Image = "~/assets/sectors/s6.jpg",
                    IsVisible = true,
                    SortOrder = 6,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "قطاع التعليم وتنمية القدرات",
                    Description = "نصمم برامج تعليمية وتدريبية وتجارب تعليمية تفاعلية تخدم أهداف التنمية وبناء القدرات وفق منهجيات حديثة.",
                    Image = "~/assets/sectors/s7.jpg",
                    IsVisible = true,
                    SortOrder = 7,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "الرعاة والشركاء الاستراتيجيون",
                    Description = "نطور شراكات استراتيجية مع الرعاة والشركاء لتعزيز القيمة المتبادلة وتحقيق الأهداف المشتركة ضمن تجارب متكاملة.",
                    Image = "~/assets/sectors/s8.jpg",
                    IsVisible = true,
                    SortOrder = 8,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new Sector
                {
                    Title = "قطاع الاستثمار",
                    Description = "نطور فرص استثمارية في قطاع الفعاليات والتجارب الترفيهية بما يدعم نمو القطاع وتحقيق عوائد مالية مستدامة.",
                    Image = "~/assets/sectors/s9.jpg",
                    IsVisible = true,
                    SortOrder = 9,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                }
            };
            context.Sectors.AddRange(sectors);
            await context.SaveChangesAsync();
        }

        // Seed Company Values
        if (!await context.CompanyValues.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            var values = new[]
            {
                new CompanyValue
                {
                    Title = "قصتنا",
                    Content = "تنطلق همم الحياة من همّة تسعى إلى تحويل الأفكار والرسائل والأهداف إلى تجارب حقيقية ذات أثر ملموس. نجمع بين التفكير الاستراتيجي والتنفيذ المنضبط؛ لنقدّم تجارب مترابطة تضع الإنسان في جوهرها، وتنتقل بالفكرة من التصوّر إلى واقع يُعاش ويُتذكّر. ولا تنتهي قيمة التجربة بانتهاء تنفيذها، بل تمتد إلى ما تحققه من نتائج، وما تتركه من أثر يمكن قياسه وتطويره والبناء عليه.",
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new CompanyValue
                {
                    Title = "رؤيتنا",
                    Content = "أن نرسّخ مكانتنا كشركة سعودية رائدة، محليًا وإقليميًا، في صناعة التجارب والفعاليات والبرامج والمشروعات المتكاملة، من خلال حلول نوعية تُثري مشاركة الجمهور، وتسهم في تعزيز جودة الحياة، وتقدّم قيمة ممتدة للشركاء والمجتمع.",
                    IsVisible = true,
                    SortOrder = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new CompanyValue
                {
                    Title = "رسالتنا",
                    Content = "نحوّل أهداف شركائنا وأفكارهم إلى تجارب وبرامج ومشروعات متكاملة ذات قيمة واضحة، تجمع بين التفكير الاستراتيجي والتصميم الإبداعي والتنفيذ المنضبط والقياس، بما يدعم تحقيق مستهدفات الجهات ويمنح الجمهور تجارب ذات معنى وأثر.",
                    IsVisible = true,
                    SortOrder = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new CompanyValue
                {
                    Title = "مهمتنا",
                    Content = "نصمّم ونطوّر وننفّذ تجارب وفعاليات وبرامج ومشروعات تنطلق من فهم الإنسان واحتياجاته، وتتكامل فيها الاستراتيجية مع المحتوى النوعي والتصميم الإبداعي والإدارة التنفيذية المنضبطة. ونحرص على أن تعزز أعمالنا مشاركة الجمهور، وتحقق قيمة مؤسسية ملموسة لشركائنا، وتنتج نتائج قابلة للقياس والتطوير والبناء عليها.",
                    IsVisible = true,
                    SortOrder = 4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                }
            };
            context.CompanyValues.AddRange(values);
            await context.SaveChangesAsync();
        }

        // Seed Process Steps
        if (!await context.ProcessSteps.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            var steps = new[]
            {
                new ProcessStep
                {
                    Title = "فهم الجمهور",
                    Subtitle = "بحث ورؤية دقيقة",
                    StepNumber = 1,
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new ProcessStep
                {
                    Title = "التخطيط الاستراتيجي",
                    Subtitle = "مفهوم قابل للقياس",
                    StepNumber = 2,
                    IsVisible = true,
                    SortOrder = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new ProcessStep
                {
                    Title = "التصميم الإبداعي",
                    Subtitle = "هوية وتجربة متكاملة",
                    StepNumber = 3,
                    IsVisible = true,
                    SortOrder = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new ProcessStep
                {
                    Title = "تطوير المحتوى",
                    Subtitle = "رسائل تخدم الهدف",
                    StepNumber = 4,
                    IsVisible = true,
                    SortOrder = 4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new ProcessStep
                {
                    Title = "التنفيذ التشغيلي",
                    Subtitle = "إدارة دقيقة للمشروع",
                    StepNumber = 5,
                    IsVisible = true,
                    SortOrder = 5,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new ProcessStep
                {
                    Title = "تقييم النتائج",
                    Subtitle = "قياس الأثر والتطوير",
                    StepNumber = 6,
                    IsVisible = true,
                    SortOrder = 6,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                }
            };
            context.ProcessSteps.AddRange(steps);
            await context.SaveChangesAsync();
        }

        // Seed Contact Info
        if (!await context.ContactInfos.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            context.ContactInfos.Add(new ContactInfo
            {
                Email = "info@himamalhayah.sa",
                Phone = "0535105327",
                Address = "جدة، حي مشرفة شارع عين الوهيط مبني 3654 الرمز البريدي23332",
                WorkingHours = "الأحد – الخميس، 9 صباحًا – 5 مساءً",
                MapEmbedUrl = "https://www.google.com/maps?q=%D8%AC%D8%AF%D8%A9%D8%8C%20%D8%AD%D9%8A%20%D9%85%D8%B4%D8%B1%D9%81%D8%A9%D8%8C%20%D8%B4%D8%A7%D8%B1%D8%B9%20%D8%B9%D9%8A%D9%86%20%D8%A7%D9%84%D9%88%D9%87%D9%8A%D8%B7&output=embed",
                IsVisible = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = user.Id
            });
            await context.SaveChangesAsync();
        }

        // Seed Social Media Links
        if (!await context.SocialMediaLinks.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            var socialLinks = new[]
            {
                new SocialMediaLink
                {
                    Platform = "X",
                    Url = "#",
                    IconSvg = "<path d=\"M18.9 3H21l-6.6 7.5L22 21h-6.3l-4.9-6.4L5.2 21H3l7.1-8.1L2.5 3h6.4l4.4 5.8L18.9 3zm-1.1 16h1.2L7.3 5H6l11.8 14z\"/>",
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "Instagram",
                    Url = "#",
                    IconSvg = "<rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"5\"/><circle cx=\"12\" cy=\"12\" r=\"4\"/><circle cx=\"17.5\" cy=\"6.5\" r=\"1\"/>",
                    IsVisible = true,
                    SortOrder = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "LinkedIn",
                    Url = "#",
                    IconSvg = "<path d=\"M4.98 3.5A2.5 2.5 0 1 1 5 8.5a2.5 2.5 0 0 1-.02-5zM3 9h4v12H3zM9 9h3.8v1.7h.05c.53-1 1.83-2.05 3.77-2.05 4.03 0 4.78 2.65 4.78 6.1V21h-4v-5.6c0-1.34-.02-3.06-1.87-3.06-1.87 0-2.16 1.46-2.16 2.96V21H9z\"/>",
                    IsVisible = true,
                    SortOrder = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "Facebook",
                    Url = "#",
                    IconSvg = "<path d=\"M13.5 9H15V6h-1.6C11.4 6 10 7.5 10 9.8V11H8v3h2v7h3v-7h2.1l.4-3H13v-1c0-.6.2-1 .5-1z\"/>",
                    IsVisible = true,
                    SortOrder = 4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "Snapchat",
                    Url = "#",
                    IconSvg = "<path d=\"M12 3.2c-2.9 0-4.9 2.1-4.9 5v1.9c0 .8 0 1.5-.2 1.9-.3.7-1.4.9-2 1.2-.4.2-.3.6 0 .8.5.3 1.1.4 1.5.5.3.1.1.5-.1.8-.3.4-.8.8-.3 1.2.6.4 1.7.2 2.4.6.6.3.8 1.2 1.9 1.4.5.1 1-.1 1.6-.1.5 0 1 .2 1.6.1 1.1-.2 1.3-1.1 1.9-1.4.7-.4 1.8-.2 2.4-.6.5-.4 0-.8-.3-1.2-.2-.3-.4-.7-.1-.8.4-.1 1-.2 1.5-.5.3-.2.4-.6 0-.8-.6-.3-1.7-.5-2-1.2-.2-.4-.2-1.1-.2-1.9V8.2c0-2.9-2-5-4.9-5z\"/>",
                    IsVisible = true,
                    SortOrder = 5,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "TikTok",
                    Url = "#",
                    IconSvg = "<path d=\"M16.5 2h-3v13.6a2.5 2.5 0 1 1-2.2-2.5v-3.2a5.6 5.6 0 1 0 5.2 5.6V9.1c1.2.8 2.6 1.3 4 1.3V7.3c-2 0-3.7-1.3-4.3-3.1-.2-.6-.2-1.4-.2-2.2z\"/>",
                    IsVisible = true,
                    SortOrder = 6,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "YouTube",
                    Url = "#",
                    IconSvg = "<path d=\"M22 8.4s-.2-1.5-.8-2.2c-.8-.9-1.7-.9-2.1-1C16.3 5 12 5 12 5s-4.3 0-7.1.2c-.4.1-1.3.1-2.1 1-.6.7-.8 2.2-.8 2.2S1.8 10.2 1.8 12v1.6c0 1.8.2 3.6.2 3.6s.2 1.5.8 2.2c.8.9 1.9.9 2.3 1 1.7.2 7 .2 7 .2s4.3 0 7.1-.2c.4-.1 1.3-.1 2.1-1 .6-.7.8-2.2.8-2.2s.2-1.8.2-3.6V12c0-1.8-.2-3.6-.2-3.6zM9.8 15.2V8.9l6 3.2-6 3.1z\"/>",
                    IsVisible = true,
                    SortOrder = 7,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new SocialMediaLink
                {
                    Platform = "WhatsApp",
                    Url = "#",
                    IconSvg = "<path d=\"M12 2C6.5 2 2 6.5 2 12c0 1.8.5 3.6 1.4 5.1L2 22l5-1.3c1.4.8 3.1 1.3 5 1.3 5.5 0 10-4.5 10-10S17.5 2 12 2zm5.6 14.2c-.2.6-1.3 1.2-1.8 1.3-.5.1-1 .1-1.7-.1-.4-.1-.9-.3-1.6-.6-2.8-1.2-4.6-4-4.7-4.2-.1-.2-1.1-1.5-1.1-2.9s.7-2 1-2.3c.2-.3.5-.3.7-.3h.5c.2 0 .4 0 .6.4.2.5.7 1.8.8 1.9.1.1.1.3 0 .5-.1.2-.1.3-.3.5-.1.2-.3.4-.4.5-.1.1-.3.3-.1.6.2.3.8 1.3 1.7 2.1 1.2 1 2.1 1.4 2.5 1.5.3.1.5.1.7-.1.2-.2.7-.8.9-1.1.2-.3.4-.2.6-.1.2.1 1.5.7 1.8.8.3.1.4.2.5.3.1.2.1.9-.1 1.5z\"/>",
                    IsVisible = true,
                    SortOrder = 8,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                }
            };
            context.SocialMediaLinks.AddRange(socialLinks);
            await context.SaveChangesAsync();
        }

        // Seed Stat Items
        if (!await context.StatItems.AnyAsync())
        {
            var user = await context.Users.FirstAsync(u => u.Id == 1);
            var stats = new[]
            {
                new StatItem
                {
                    Title = "مشروع ناجح",
                    Value = "320+",
                    Description = "مشروع تنفيذي ناجح",
                    IsVisible = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new StatItem
                {
                    Title = "ميليون زائر",
                    Value = "320+",
                    Description = "مليون زائر مستفيد",
                    IsVisible = true,
                    SortOrder = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new StatItem
                {
                    Title = "شريك استراتيجي",
                    Value = "50+",
                    Description = "شريك استراتيجي",
                    IsVisible = true,
                    SortOrder = 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                },
                new StatItem
                {
                    Title = "دولة",
                    Value = "15+",
                    Description = "دولة عربية وعالمية",
                    IsVisible = true,
                    SortOrder = 4,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = user.Id
                }
            };
            context.StatItems.AddRange(stats);
            await context.SaveChangesAsync();
        }
    }
}
