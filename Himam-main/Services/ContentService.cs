using System.Security.Claims;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Services;

public interface IContentService
{
    Task<IReadOnlyList<Page>> GetPagesAsync();
    Task<Page?> GetPageBySlugAsync(string slug, bool publishedOnly = false);
    Task<Page> SavePageAsync(string slug, Page input, int userId, bool canPublish);
    Task<IReadOnlyList<News>> GetNewsAsync(string? status = null);
    Task<News?> GetNewsByIdAsync(int id);
    Task<News> SaveNewsAsync(News input, int userId, bool canPublish, int? id = null);
    Task<bool> DeleteNewsAsync(int id, int userId);
    Task<IReadOnlyList<Setting>> GetSettingsAsync();
    Task SaveSettingsAsync(Dictionary<string, string> values);
    Task<IReadOnlyList<ServiceCategory>> GetServiceCategoriesAsync();
    Task<ServiceCategory> SaveServiceCategoryAsync(ServiceCategory input, int? id = null);
    Task<bool> DeleteServiceCategoryAsync(int id);
}

public class ContentService : IContentService
{
    private readonly HimanAlhayahContext _context;
    private readonly IAuditLogService _auditLog;

    public ContentService(HimanAlhayahContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<Page>> GetPagesAsync()
        => await _context.Pages.OrderBy(p => p.Title).ToListAsync();

    public async Task<Page?> GetPageBySlugAsync(string slug, bool publishedOnly = false)
    {
        var query = _context.Pages.Where(p => p.Slug == slug);
        if (publishedOnly)
            query = query.Where(p => p.Status == "published");
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Page> SavePageAsync(string slug, Page input, int userId, bool canPublish)
    {
        var page = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);
        var isNew = page is null;

        page ??= new Page { Slug = slug, CreatedAt = DateTime.Now };

        page.Title = input.Title;
        page.ContentAr = input.ContentAr;
        page.MetaTitle = input.MetaTitle;
        page.MetaDescription = input.MetaDescription;
        page.Image = input.Image;
        page.UserId = userId;
        page.UpdatedAt = DateTime.Now;

        if (canPublish)
            page.Status = string.IsNullOrWhiteSpace(input.Status) ? "draft" : input.Status;
        else if (isNew)
            page.Status = "draft";
        else if (page.Status != "published")
            page.Status = "draft";

        if (isNew)
            _context.Pages.Add(page);

        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            isNew ? "PageCreated" : "PageUpdated",
            userId,
            success: true,
            details: $"صفحة: {slug}",
            changes: new { page.Id, page.Slug, page.Status });

        return page;
    }

    public async Task<IReadOnlyList<News>> GetNewsAsync(string? status = null)
    {
        var query = _context.News.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(n => n.Status == status);
        return await query.OrderByDescending(n => n.UpdatedAt ?? n.CreatedAt).ToListAsync();
    }

    public async Task<News?> GetNewsByIdAsync(int id)
        => await _context.News.FindAsync(id);

    public async Task<News> SaveNewsAsync(News input, int userId, bool canPublish, int? id = null)
    {
        News news;
        var isNew = !id.HasValue;

        if (isNew)
        {
            news = new News { CreatedAt = DateTime.Now };
            _context.News.Add(news);
        }
        else
        {
            var newsId = id ?? throw new InvalidOperationException("معرّف الخبر مطلوب.");
            news = await _context.News.FindAsync(newsId)
                ?? throw new KeyNotFoundException("الخبر غير موجود.");
        }

        news.Title = input.Title;
        news.Slug = string.IsNullOrWhiteSpace(input.Slug)
            ? Slugify(input.Title ?? "news")
            : input.Slug;
        news.ContentAr = input.ContentAr;
        news.ContentEn = input.ContentEn;
        news.Image = input.Image;
        news.MetaTitle = input.MetaTitle;
        news.MetaDescription = input.MetaDescription;
        news.UserId = userId;
        news.UpdatedAt = DateTime.Now;

        if (canPublish && !string.IsNullOrWhiteSpace(input.Status))
            news.Status = input.Status;
        else if (isNew)
            news.Status = "draft";
        else if (news.Status != "published")
            news.Status = "draft";

        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            isNew ? "NewsCreated" : "NewsUpdated",
            userId,
            success: true,
            details: news.Title,
            changes: new { news.Id, news.Status });

        if (news.Status == "published")
        {
            await _auditLog.LogAsync(
                "ContentPublished",
                userId,
                success: true,
                details: $"نشر: {news.Title}",
                changes: new { news.Id, Type = "News" });
        }

        return news;
    }

    public async Task<bool> DeleteNewsAsync(int id, int userId)
    {
        var news = await _context.News.FindAsync(id);
        if (news is null)
            return false;

        _context.News.Remove(news);
        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            "ContentDeleted",
            userId,
            success: true,
            details: $"حذف خبر: {news.Title}",
            changes: new { news.Id });

        return true;
    }

    public async Task<IReadOnlyList<Setting>> GetSettingsAsync()
        => await _context.Settings.OrderBy(s => s.GroupName).ThenBy(s => s.KeyName).ToListAsync();

    public async Task SaveSettingsAsync(Dictionary<string, string> values)
    {
        var settings = await _context.Settings.ToListAsync();
        foreach (var (key, value) in values)
        {
            var setting = settings.FirstOrDefault(s => s.KeyName == key);
            if (setting is null)
            {
                _context.Settings.Add(new Setting
                {
                    KeyName = key,
                    Value = value,
                    GroupName = "General",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            else
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.Now;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ServiceCategory>> GetServiceCategoriesAsync()
        => await _context.ServiceCategories.OrderBy(s => s.SortOrder).ToListAsync();

    public async Task<ServiceCategory> SaveServiceCategoryAsync(ServiceCategory input, int? id = null)
    {
        ServiceCategory item;
        if (id.HasValue)
        {
            item = await _context.ServiceCategories.FindAsync(id.Value)
                ?? throw new KeyNotFoundException("الخدمة غير موجودة.");
        }
        else
        {
            item = new ServiceCategory { CreatedAt = DateTime.Now };
            _context.ServiceCategories.Add(item);
        }

        item.Title = input.Title;
        item.Description = input.Description;
        item.SortOrder = input.SortOrder ?? 0;
        item.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteServiceCategoryAsync(int id)
    {
        var item = await _context.ServiceCategories.FindAsync(id);
        if (item is null)
            return false;
        _context.ServiceCategories.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string Slugify(string text)
    {
        var slug = text.Trim().ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^\w\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[\s_-]+", "-");
        return slug.Trim('-');
    }
}

public static class CurrentUserExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    public static bool CanPublish(this ClaimsPrincipal user)
        => user.IsInRole("Super Admin") || user.IsInRole("Site Manager");
}
