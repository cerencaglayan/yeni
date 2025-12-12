using System.Net.Mail;
using Microsoft.Extensions.Options;
using yeni.Domain.DTO.Requests;
using yeni.Infrastructure.Configuration;

namespace yeni.Application.Services;

public class MailService:IMailService
{
    private readonly GmailConfig _config;
    
    public MailService(IOptions<GmailConfig> gmailConfig)
    {
        _config = gmailConfig.Value;
    }

    public async Task SendMailAsync(SendMailRequest request)
    {
        MailMessage mailMessage = new MailMessage()
        {
            From = new MailAddress(_config.Email),
            Subject = request.Subject,
            Body = request.Body,
        };
        
        mailMessage.To.Add(request.Recipient);
        
        using var smptClient = new SmtpClient();
        smptClient.Host ="smtp.gmail.com";
        smptClient.Port = 587;
        smptClient.Credentials = new System.Net.NetworkCredential(_config.Email, _config.Password);
        smptClient.EnableSsl = true;

        try
        {
            await smptClient.SendMailAsync(mailMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
        

    }
}