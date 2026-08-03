using Himam_main.Authorization;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManageAdmins)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class InvitationsController : Controller
{
    private readonly IInvitationService _invitations;
    private readonly IEmailService _email;
    private readonly IConfiguration _configuration;

    public InvitationsController(
        IInvitationService invitations,
        IEmailService email,
        IConfiguration configuration)
    {
        _invitations = invitations;
        _email = email;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] InviteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { error = "البريد الإلكتروني مطلوب." });

        if (!_email.IsConfigured())
            return BadRequest(new { error = "إعدادات SMTP غير مكتملة. تأكد من Password في User Secrets." });

        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var baseUrl = _configuration["SiteSettings:PublicBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                baseUrl = $"{Request.Scheme}://{Request.Host}";

            var invitation = await _invitations.SendInvitationAsync(
                request.Email,
                request.FullName,
                request.Role ?? "manager",
                userId.Value,
                baseUrl);

            return Json(new
            {
                success = true,
                message = "تم إرسال رابط الدعوة إلى البريد الإلكتروني.",
                invitation.Email,
                invitation.RoleName
            });
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException is not null)
                message = ex.InnerException.Message;
            return BadRequest(new { error = message });
        }
    }

    public class InviteRequest
    {
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
