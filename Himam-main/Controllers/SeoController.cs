using Himam_main.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;

namespace Himam_main.Controllers;

public class SeoController : Controller
{
    private readonly HimanAlhayahContext _context;
    private readonly IConfiguration _configuration;

    public SeoController(HimanAlhayahContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("/robots.txt")]
    public ContentResult Robots()
    {
        var baseUrl = _configuration["SiteSettings:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        var content = $"""
            User-agent: *
            Allow: /
            Disallow: /Admin/
            Disallow: /admin/

            Sitemap: {baseUrl.TrimEnd('/')}/sitemap.xml
            """;

        return Content(content, "text/plain", Encoding.UTF8);
    }

    [HttpGet("/sitemap.xml")]
    public async Task<ContentResult> Sitemap()
    {
        var baseUrl = (_configuration["SiteSettings:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}").TrimEnd('/');

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlset = new XElement(ns + "urlset");

        var staticPages = new[]
        {
            ("", "daily", "1.0"),
            ("/User/Home/About", "monthly", "0.8"),
            ("/User/Home/Contact", "monthly", "0.7"),
            ("/User/Home/News", "daily", "0.9")
        };

        foreach (var (path, freq, priority) in staticPages)
        {
            urlset.Add(new XElement(ns + "url",
                new XElement(ns + "loc", baseUrl + path),
                new XElement(ns + "changefreq", freq),
                new XElement(ns + "priority", priority)));
        }

        var newsItems = await _context.News
            .Where(n => n.Status == "published")
            .Select(n => new { n.Slug, n.UpdatedAt })
            .ToListAsync();

        foreach (var item in newsItems)
        {
            if (string.IsNullOrWhiteSpace(item.Slug))
                continue;

            urlset.Add(new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/User/Home/NewsSingle/{item.Slug}"),
                new XElement(ns + "lastmod", (item.UpdatedAt ?? DateTime.Now).ToString("yyyy-MM-dd")),
                new XElement(ns + "changefreq", "weekly"),
                new XElement(ns + "priority", "0.7")));
        }

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), urlset);
        return Content(doc.ToString(), "application/xml", Encoding.UTF8);
    }
}
