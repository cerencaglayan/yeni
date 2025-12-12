using yeni.Domain.Entities.Base;

namespace yeni.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User,int>
{
    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<User?>> GetByBirthdayAsync(DateTime date, CancellationToken cancellationToken = default);

}