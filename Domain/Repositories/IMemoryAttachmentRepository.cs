using yeni.Domain.Entities;
using yeni.Domain.Entities.Base;

namespace yeni.Domain.Repositories;

public interface IMemoryAttachmentRepository: IBaseRepository<MemoryAttachment, int>
{
    Task<MemoryAttachment?> GetByMemoryAndAttachmentIdAsync(int memoryId, int attachmentId, CancellationToken cancellationToken = default);
    Task<List<MemoryAttachment>> GetByMemoryIdAsync(int memoryId, CancellationToken cancellationToken = default);
    Task<MemoryAttachment> CreateAsync(MemoryAttachment memoryAttachment, CancellationToken cancellationToken = default);
    Task UpdateAsync(MemoryAttachment memoryAttachment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    
}