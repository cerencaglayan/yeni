using yeni.Domain.Entities.Base;

namespace yeni.Domain.Repository;

public interface IUserRepository : IBaseRepository<User,int>
{
    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

}