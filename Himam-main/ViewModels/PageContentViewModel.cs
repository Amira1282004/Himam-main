using System.Text.Json;

namespace Himam_main.ViewModels;

public class PageContentViewModel
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public Dictionary<string, string> Fields { get; set; } = new();

    public static PageContentViewModel FromPage(Models.Page page)
    {
        var fields = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(page.ContentAr))
        {
            try
            {
                fields = JsonSerializer.Deserialize<Dictionary<string, string>>(page.ContentAr)
                    ?? new Dictionary<string, string>();
            }
            catch
            {
                fields["body"] = page.ContentAr;
            }
        }

        return new PageContentViewModel
        {
            Slug = page.Slug ?? "",
            Title = page.Title ?? "",
            MetaTitle = page.MetaTitle,
            MetaDescription = page.MetaDescription,
            Fields = fields
        };
    }

    public string Get(string key, string fallback = "")
        => Fields.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : fallback;
}
