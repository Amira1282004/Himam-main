using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Himam_main.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsConfigured()
    {
        var host = _configuration["SmtpSettings:Host"];
        var from = _configuration["SmtpSettings:FromEmail"];
        var user = _configuration["SmtpSettings:Username"];
        var pass = _configuration["SmtpSettings:Password"];
        return !string.IsNullOrWhiteSpace(host)
            && !string.IsNullOrWhiteSpace(from)
            && !string.IsNullOrWhiteSpace(user)
            && !string.IsNullOrWhiteSpace(pass);
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (!IsConfigured())
            throw new InvalidOperationException("إعدادات البريد غير مكتملة. تأكد من Host وUsername وPassword في User Secrets.");

        var host = _configuration["SmtpSettings:Host"]!;
        var port = int.TryParse(_configuration["SmtpSettings:Port"], out var p) ? p : 587;
        var username = _configuration["SmtpSettings:Username"]!.Trim();
        var password = _configuration["SmtpSettings:Password"]!;
        var fromEmail = _configuration["SmtpSettings:FromEmail"]!.Trim();
        var fromName = _configuration["SmtpSettings:FromName"] ?? "همم الحياة";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail.Trim()));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();

        try
        {
            var socketOptions = ResolveSocketOptions(port);
            await client.ConnectAsync(host, port, socketOptions);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل إرسال البريد إلى {Email} عبر {Host}:{Port}", toEmail, host, port);
            throw new InvalidOperationException(BuildFriendlyError(ex, host, port), ex);
        }
    }

    private SecureSocketOptions ResolveSocketOptions(int port)
    {
        var mode = _configuration["SmtpSettings:SecurityMode"]?.Trim().ToLowerInvariant();
        if (mode == "ssl")
            return SecureSocketOptions.SslOnConnect;
        if (mode == "starttls")
            return SecureSocketOptions.StartTls;
        if (mode == "none")
            return SecureSocketOptions.None;

        // Titan: 465 = SSL، 587 = STARTTLS
        return port switch
        {
            465 => SecureSocketOptions.SslOnConnect,
            587 => SecureSocketOptions.StartTls,
            _ => SecureSocketOptions.Auto
        };
    }

    private static string BuildFriendlyError(Exception ex, string host, int port)
    {
        var detail = ex.Message;
        if (ex.InnerException is not null)
            detail = ex.InnerException.Message;

        if (detail.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
            detail.Contains("535", StringComparison.OrdinalIgnoreCase))
        {
            return "فشل المصادقة مع خادم البريد. تحقق من Username وPassword (كلمة مرور البريد وليس كلمة مرور الموقع).";
        }

        return $"تعذّر إرسال البريد عبر {host}:{port}. {detail}";
    }
}
