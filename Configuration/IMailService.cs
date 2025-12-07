using yeni.Domain.DTO.Requests;

namespace yeni.Configuration;

public interface IMailService
{
    Task SendMailAsync(SendMailRequest request);
}