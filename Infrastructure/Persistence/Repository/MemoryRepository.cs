using Microsoft.EntityFrameworkCore;
using yeni.Data;
using yeni.Domain.Entities;
using yeni.Domain.Repositories;

namespace yeni.Infrastructure.Persistence.Repository;

public class MemoryRepository : IMemoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MemoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Memory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Memories
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Memory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Memories
            .Where(m => !m.IsDeleted)
            .Include(m => m.MemoryAttachments.Where(mp => !mp.IsDeleted))
                .ThenInclude(mp => mp.Attachment)
            .OrderByDescending(m => m.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Memory?> GetByIdWithAttachmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Memories
            .Include(m => m.MemoryAttachments.Where(mp => !mp.IsDeleted && !mp.Attachment.IsDeleted))
                .ThenInclude(mp => mp.Attachment)
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Memory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Memories
            .Include(m => m.MemoryAttachments.Where(mp => !mp.IsDeleted && !mp.Attachment.IsDeleted))
                .ThenInclude(mp => mp.Attachment)
            .Where(m => m.UserId == userId && !m.IsDeleted)
            .OrderByDescending(m => m.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Memory> CreateAsync(Memory memory, CancellationToken cancellationToken = default)
    {
        _dbContext.Memories.Add(memory);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return memory;
    }

    public async Task UpdateAsync(Memory memory, CancellationToken cancellationToken = default)
    {
        memory.ModifiedAt = DateTime.UtcNow;
        _dbContext.Memories.Update(memory);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var memory = await GetByIdAsync(id, cancellationToken);
        if (memory != null)
        {
            memory.IsDeleted = true;
            memory.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}