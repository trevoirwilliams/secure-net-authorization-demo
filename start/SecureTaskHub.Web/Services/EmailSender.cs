using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SecureTaskHub.Web.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var smtpHost = smtpSettings["Host"] ?? "localhost";
        var smtpPort = int.Parse(smtpSettings["Port"] ?? "25");
        var fromEmail = smtpSettings["FromEmail"] ?? "noreply@securetaskhub.local";
        var fromName = smtpSettings["FromName"] ?? "SecureTaskHub";

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = false; // Papercut doesn't use SSL
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", email, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }
}
