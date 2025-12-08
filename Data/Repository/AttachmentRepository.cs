using Microsoft.EntityFrameworkCore;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repository;

namespace yeni.Data.Repository;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AttachmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Attachment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attachments
            .Where(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Attachment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attachments
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Attachment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attachments
            .Where(p => p.UploadedByUserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Attachment> CreateAsync(Attachment attachment, CancellationToken cancellationToken = default)
    {
        _dbContext.Attachments.Add(attachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return attachment;
    }

    // ✅ MEMORY'E BAĞLAMA
    public async Task<MemoryAttachment> AddAttachmentToMemoryAsync(
        int memoryId,
        int attachmentId,
        int displayOrder,
        string? caption = null)
    {
        var entity = new MemoryAttachment
        {
            MemoryId = memoryId,
            AttachmentId = attachmentId,
            DisplayOrder = displayOrder,
            Caption = caption
        };

        _dbContext.MemoryAttachments.Add(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    // ✅ MEMORY'DEN KOPARMA
    public async Task<bool> RemoveAttachmentFromMemoryAsync(int memoryId, int attachmentId)
    {
        var entity = await _dbContext.MemoryAttachments
            .FirstOrDefaultAsync(x =>
                x.MemoryId == memoryId &&
                x.AttachmentId == attachmentId &&
                !x.IsDeleted);

        if (entity == null)
            return false;

        entity.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    // ✅ DOSYAYI DB'DEN SİL (B2 SİLME SERVICE'TE)
    public async Task<bool> DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await _dbContext.Attachments
            .FirstOrDefaultAsync(x => x.Id == attachmentId && !x.IsDeleted);

        if (attachment == null)
            return false;

        attachment.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    // ✅ SIRALAMA & CAPTION
    public async Task UpdateMemoryAttachmentAsync(
        int memoryAttachmentId,
        int? displayOrder,
        string? caption)
    {
        var entity = await _dbContext.MemoryAttachments
            .FirstOrDefaultAsync(x => x.Id == memoryAttachmentId && !x.IsDeleted);

        if (entity == null)
            throw new Exception("MemoryAttachment not found");

        if (displayOrder.HasValue)
            entity.DisplayOrder = displayOrder.Value;

        if (caption != null)
            entity.Caption = caption;

        await _dbContext.SaveChangesAsync();
    }
}
