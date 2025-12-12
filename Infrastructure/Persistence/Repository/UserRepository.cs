using Microsoft.EntityFrameworkCore;
using yeni.Data;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repositories;

namespace yeni.Infrastructure.Persistence.Repository;

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

    public async Task<List<User?>> GetByBirthdayAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<User>()
            .Where(q =>
                q.Birthday.HasValue &&              
                !q.IsDeleted &&                  
                q.Birthday.Value.Day == date.Day && 
                q.Birthday.Value.Month == date.Month
            )
            .ToListAsync(cancellationToken);
    }

    
    
    
    
}