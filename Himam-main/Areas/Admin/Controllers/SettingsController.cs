using Himam_main.Authorization;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManageSettings)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class SettingsController : Controller
{
    private readonly IContentService _content;
    private readonly IAuditLogService _auditLog;

    public SettingsController(IContentService content, IAuditLogService auditLog)
    {
        _content = content;
        _auditLog = auditLog;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _content.GetSettingsAsync();
        return Json(settings.ToDictionary(s => s.KeyName ?? "", s => s.Value ?? ""));
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] Dictionary<string, string> values)
    {
        var userId = User.GetUserId();
        await _content.SaveSettingsAsync(values);

        if (userId.HasValue)
        {
            await _auditLog.LogAsync(
                "SettingsChanged",
                userId,
                success: true,
                details: "تحديث الإعدادات العامة",
                changes: values.Keys);
        }

        return Json(new { success = true });
    }
}
