using Microsoft.AspNetCore.Http;
using yeni.Domain.Entities.Base;

namespace yeni.Domain.Repository;

public interface IAttachmentRepository : IBaseRepository<Attachment, int>
{
    // ✅ SADECE METADATA KAYDI (B2 sonrası)
    Task<Attachment> CreateAsync(Attachment attachment, CancellationToken cancellationToken = default);

    // ✅ Kullanıcının dosyaları
    Task<List<Attachment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    // ✅ Memory'e bağlama işlemleri
    Task<MemoryAttachment> AddAttachmentToMemoryAsync(int memoryId, int attachmentId, int displayOrder, string? caption = null);
    Task<bool> RemoveAttachmentFromMemoryAsync(int memoryId, int attachmentId);

    // ✅ Kalıcı silme (DB + B2)
    Task<bool> DeleteAttachmentAsync(int attachmentId);

    // ✅ Memory içindeki sıralama/güncelleme
    Task UpdateMemoryAttachmentAsync(int memoryAttachmentId, int? displayOrder, string? caption);
}