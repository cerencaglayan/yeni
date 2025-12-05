using Microsoft.EntityFrameworkCore;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repository;

namespace yeni.Data.Repository;

public class UserRepository(ApplicationDbContext dbContext):IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .Where(q => q.Id == id && !q.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .Where(q => !q.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .Where(q => q.Name == name && !q.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    
    
    
    
}