using Microsoft.EntityFrameworkCore;
using yeni.Domain.Entities;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repositories;
using yeni.Infrastructure.Persistence;

namespace yeni.Data.Repository;

public class MemoryAttachmentRepository : IMemoryAttachmentRepository
{
     private readonly ApplicationDbContext _dbContext;

    public MemoryAttachmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MemoryAttachment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MemoryAttachments
            .Include(mp => mp.Memory)
            .Include(mp => mp.Attachment)
            .Where(mp => mp.Id == id && !mp.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MemoryAttachment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.MemoryAttachments
            .Include(mp => mp.Memory)
            .Include(mp => mp.Attachment)
            .Where(mp => !mp.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemoryAttachment?> GetByMemoryAndAttachmentIdAsync(int memoryId, int AttachmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MemoryAttachments
            .Where(mp => mp.MemoryId == memoryId && mp.AttachmentId == AttachmentId && !mp.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MemoryAttachment>> GetByMemoryIdAsync(int memoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MemoryAttachments
            .Include(mp => mp.Attachment)
            .Where(mp => mp.MemoryId == memoryId && !mp.IsDeleted)
            .OrderBy(mp => mp.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemoryAttachment> CreateAsync(MemoryAttachment memoryAttachment, CancellationToken cancellationToken = default)
    {
        _dbContext.MemoryAttachments.Add(memoryAttachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return memoryAttachment;
    }

    public async Task UpdateAsync(MemoryAttachment memoryAttachment, CancellationToken cancellationToken = default)
    {
        memoryAttachment.ModifiedAt = DateTime.UtcNow;
        _dbContext.MemoryAttachments.Update(memoryAttachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var memoryAttachment = await GetByIdAsync(id, cancellationToken);
        if (memoryAttachment != null)
        {
            memoryAttachment.IsDeleted = true;
            memoryAttachment.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }   
}