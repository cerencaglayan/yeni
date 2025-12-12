using yeni.Domain.Entities;


namespace yeni.Domain.Repositories;

public interface IAttachmentRepository : IBaseRepository<Attachment, int>
{
    Task<Attachment> CreateAsync(Attachment attachment, CancellationToken cancellationToken = default);

    Task<List<Attachment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<MemoryAttachment> AddAttachmentToMemoryAsync(int memoryId, int attachmentId, int displayOrder, string? caption = null);
    Task<bool> RemoveAttachmentFromMemoryAsync(int memoryId, int attachmentId);

    Task<bool> DeleteAttachmentAsync(int attachmentId);

    Task UpdateMemoryAttachmentAsync(int memoryAttachmentId, int? displayOrder, string? caption);
}