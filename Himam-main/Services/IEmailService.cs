namespace Himam_main.Services;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
    bool IsConfigured();
}
