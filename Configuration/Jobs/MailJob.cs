using yeni.Domain.DTO.Requests;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repository;

namespace yeni.Configuration;

public class MailJob(IUserRepository userRepository,IMailService mailService)
{
   
    
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var birthdayUsers = await CheckBirthdays(cancellationToken);
        
        if (!birthdayUsers.Any())
            return;
        
        var allUsers = await userRepository.GetAllAsync(cancellationToken);
        
        var usersToNotify = allUsers
            .Where(u => birthdayUsers.All(b => b!.Id != u.Id))
            .ToList();
        
        foreach (var birthdayUser in birthdayUsers)
        {
            var mailBody = PrepareMailBody(birthdayUser!);

            foreach (var receiver in usersToNotify)
            {
                if (receiver.Email is null)
                {
                    continue;
                }
                
                SendMailRequest request = new SendMailRequest();
                request.Recipient = receiver.Email;
                request.Body = mailBody;
                request.Subject = "🎉 Doğum Günü Hatırlatması";
                
                await mailService.SendMailAsync(request);
            }
        }
        
    }

    private async Task<List<User?>> CheckBirthdays(CancellationToken cancellationToken)
    {
        return await userRepository.GetByBirthdayAsync(DateTime.Today, cancellationToken);

    }

    private string PrepareMailBody(User user)
    {
        return $"""
                🎂 Bugün uygulamadaki kullanıcılardan birinin doğum günü!

                🧑 İyi ki doğdun {user.Name} !
                
                {user.Birthday.Value.Year - DateTime.Today.Year}.yaşın sana uğur getirsin!

                Kendisine güzel bir kutlama iletmeyi unutma! 🎉
                """;
    }

    
    
    
    
}