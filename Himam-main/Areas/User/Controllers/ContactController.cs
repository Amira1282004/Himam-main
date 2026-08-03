using Himam_main.Data;
using Himam_main.Models;
using Himam_main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Himam.Areas.User.Controllers;

[Area("User")]
public class ContactController : Controller
{
    private readonly HimanAlhayahContext _context;
    private readonly IAuditLogService _auditLogService;

    public ContactController(HimanAlhayahContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        string name,
        string email,
        string? phone,
        string? organization,
        string? sector,
        string message)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
        {
            TempData["ContactError"] = "يرجى تعبئة الحقول المطلوبة.";
            return RedirectToAction("Contact", "Home");
        }

        var fullMessage = message.Trim();
        if (!string.IsNullOrWhiteSpace(organization))
            fullMessage = $"[الجهة: {organization.Trim()}]\n{fullMessage}";
        if (!string.IsNullOrWhiteSpace(sector))
            fullMessage = $"[القطاع: {sector.Trim()}]\n{fullMessage}";

        var contact = new Contact
        {
            Name = name.Trim(),
            Email = email.Trim(),
            Phone = phone?.Trim(),
            Message = fullMessage,
            Status = "new",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            "ContactFormSubmitted",
            userId: null,
            success: true,
            details: $"رسالة تواصل جديدة من {contact.Name}",
            changes: new { contact.Id, contact.Email },
            httpContext: HttpContext);

        TempData["ContactSuccess"] = "تم إرسال رسالتك بنجاح. سيتواصل معك فريقنا قريباً.";
        return RedirectToAction("Contact", "Home");
    }
}
