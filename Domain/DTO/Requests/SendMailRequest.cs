using System.Net.Mail;

namespace yeni.Domain.DTO.Requests;

public class SendMailRequest
{
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}