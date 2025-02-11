using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Identity
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

       public async Task SendEmailAsync(string toEmail, string subject, string message)
{
    try
    {
      //  var emailSettings = _configuration.GetSection("EmailSettings");

        var smtpServer = "smtp.gmail.com"; 
        var smtpPort = 587;
        var senderEmail ="kharviashrith06@gmail.com"; 
        var senderPassword = "lafy wisn eofa ysuy"; 
        var enableSSL = true;

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
        {
            throw new Exception("SMTP configuration is missing in appsettings.json.");
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Your App", senderEmail));
        email.To.Add(new MailboxAddress(toEmail, toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = message,
            HtmlBody = message
        };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpServer, smtpPort, enableSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        
        _logger.LogInformation($"Email sent successfully to {toEmail}.");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Unexpected error sending email: {ex.Message}");
        throw;
    }
}

    }
}
