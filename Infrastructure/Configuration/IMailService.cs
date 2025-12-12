using yeni.Domain.DTO.Requests;

namespace yeni.Infrastructure.Configuration;

public interface IMailService
{
    Task SendMailAsync(SendMailRequest request);
}