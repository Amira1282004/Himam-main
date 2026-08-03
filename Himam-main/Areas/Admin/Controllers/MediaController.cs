using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.UploadMedia)]
[Route("Admin/Api/[controller]")]
[RequestSizeLimit(10 * 1024 * 1024)]
public class MediaController : Controller
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".mp4", ".pdf"
    };

    private readonly HimanAlhayahContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IAuditLogService _auditLog;

    public MediaController(HimanAlhayahContext context, IWebHostEnvironment env, IAuditLogService auditLog)
    {
        _context = context;
        _env = env;
        _auditLog = auditLog;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _context.Media
            .OrderByDescending(m => m.UploadedAt)
            .Take(100)
            .Select(m => new { m.Id, m.FileName, m.FilePath, m.FileType, m.FileSize, m.UploadedAt })
            .ToListAsync();
        return Json(items);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "لم يُرفَع ملف." });

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
            return BadRequest(new { error = "نوع الملف غير مسموح." });

        var userId = User.GetUserId();
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", DateTime.UtcNow.ToString("yyyy-MM"));
        Directory.CreateDirectory(uploadsDir);

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var physicalPath = Path.Combine(uploadsDir, safeName);
        await using (var stream = System.IO.File.Create(physicalPath))
            await file.CopyToAsync(stream);

        var relativePath = "/" + Path.GetRelativePath(_env.WebRootPath, physicalPath).Replace('\\', '/');
        var medium = new Medium
        {
            FileName = Path.GetFileName(file.FileName),
            FilePath = relativePath,
            FileType = file.ContentType,
            FileSize = (int)file.Length,
            UploadedAt = DateTime.Now,
            UserId = userId
        };

        _context.Media.Add(medium);
        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            "FileUploaded",
            userId,
            success: true,
            details: medium.FileName,
            changes: new { medium.Id, medium.FilePath });

        return Json(new { success = true, medium.Id, url = relativePath });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.ManagePages)]
    public async Task<IActionResult> Delete(int id)
    {
        var medium = await _context.Media.FindAsync(id);
        if (medium is null)
            return NotFound();

        var physical = Path.Combine(_env.WebRootPath, medium.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(physical))
            System.IO.File.Delete(physical);

        _context.Media.Remove(medium);
        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            "FileDeleted",
            User.GetUserId(),
            success: true,
            details: medium.FileName,
            changes: new { medium.Id });

        return Json(new { success = true });
    }
}
