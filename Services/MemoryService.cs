using yeni.Domain.DTO.Requests;
using yeni.Domain.DTO.Responses;
using yeni.Domain.Entities.Base;
using yeni.Domain.Repository;

namespace yeni.Configuration;

public class MemoryService
{
    private readonly IMemoryRepository _memoryRepo;
    private readonly IAttachmentRepository _attachmentRepo;
    private readonly IMemoryAttachmentRepository _memoryAttachmentRepo;
    private readonly B2StorageService _storageService;

    public MemoryService(
        IMemoryRepository memoryRepo,
        IAttachmentRepository attachmentRepo,
        IMemoryAttachmentRepository memoryAttachmentRepo,
        B2StorageService storageService)
    {
        _memoryRepo = memoryRepo;
        _attachmentRepo = attachmentRepo;
        _memoryAttachmentRepo = memoryAttachmentRepo;
        _storageService = storageService;
    }

    /// <summary>
    /// Yeni memory oluşturur ve dosyaları yükler
    /// </summary>
    public async Task<MemoryResponse> CreateMemoryWithAttachmentsAsync(
        CreateMemoryRequest request,
        int userId,
        CancellationToken ct = default)
    {
        // 1. Memory'yi oluştur
        var memory = new Memory
        {
            Description = request.Description,
            Date = request.Date,
            UserId = userId
        };

        memory = await _memoryRepo.CreateAsync(memory, ct);

        // 2. Dosyaları yükle ve attachment'ları oluştur
        var attachments = new List<Attachment>();
        if (request.Files != null && request.Files.Any())
        {
            var displayOrder = 0;
            foreach (var file in request.Files)
            {
                var filePath = await _storageService.UploadFileAsync(file, userId, ct);

                // Attachment kaydı oluştur
                var attachment = Attachment.Create(
                    fileName: file.FileName,
                    filePath: filePath,
                    contentType: file.ContentType,
                    fileSize: file.Length,
                    uploadedByUserId: userId
                );

                attachment = await _attachmentRepo.CreateAsync(attachment, ct);
                attachments.Add(attachment);

                // Memory-Attachment ilişkisini oluştur
                await _memoryAttachmentRepo.CreateAsync(
                    MemoryAttachment.Create(
                        memoryId: memory.Id,
                        attachmentId: attachment.Id,
                        displayOrder: displayOrder++,
                        caption: request.Captions?.ElementAtOrDefault(displayOrder - 1)
                    ), ct);
            }
        }

        return await GetMemoryByIdAsync(memory.Id, ct);
    }

    /// <summary>
    /// Memory'yi attachments ile birlikte getirir
    /// </summary>
    public async Task<MemoryResponse> GetMemoryByIdAsync(int memoryId, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdWithAttachmentsAsync(memoryId, ct);
        if (memory == null)
            throw new Exception("Memory not found");

        return MapToDto(memory);
    }

    /// <summary>
    /// Kullanıcının tüm memory'lerini getirir
    /// </summary>
    public async Task<List<MemoryResponse>> GetUserMemoriesAsync(int userId, CancellationToken ct = default)
    {
        var memories = await _memoryRepo.GetByUserIdAsync(userId, ct);
        return memories.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Memory'ye yeni dosyalar ekler
    /// </summary>
    public async Task<MemoryResponse> AddAttachmentsToMemoryAsync(
        int memoryId,
        List<IFormFile> files,
        int userId,
        List<string?>? captions = null,
        CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null)
            throw new Exception("Memory not found");

        if (memory.UserId != userId)
            throw new UnauthorizedAccessException("You can only add attachments to your own memories");

        // Mevcut en yüksek display order'ı bul
        var existingAttachments = await _memoryAttachmentRepo.GetByMemoryIdAsync(memoryId, ct);
        var maxDisplayOrder = existingAttachments.Any() ? existingAttachments.Max(x => x.DisplayOrder) : -1;

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            
            // B2'ye yükle
            var filePath = await _storageService.UploadFileAsync(file, userId, ct);

            // Attachment kaydı oluştur
            var attachment = Attachment.Create(
                fileName: file.FileName,
                filePath: filePath,
                contentType: file.ContentType,
                fileSize: file.Length,
                uploadedByUserId: userId
            );

            attachment = await _attachmentRepo.CreateAsync(attachment, ct);

            // Memory-Attachment ilişkisini oluştur
            await _memoryAttachmentRepo.CreateAsync(
                MemoryAttachment.Create(
                    memoryId: memoryId,
                    attachmentId: attachment.Id,
                    displayOrder: ++maxDisplayOrder,
                    caption: captions?.ElementAtOrDefault(i)
                ), ct);
        }

        return await GetMemoryByIdAsync(memoryId, ct);
    }

    /// <summary>
    /// Memory'den attachment'ı kaldırır ve B2'den siler
    /// </summary>
    public async Task<bool> RemoveAttachmentFromMemoryAsync(
        int memoryId,
        int attachmentId,
        int userId,
        CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null || memory.UserId != userId)
            throw new UnauthorizedAccessException();

        var attachment = await _attachmentRepo.GetByIdAsync(attachmentId, ct);
        if (attachment == null)
            return false;

        // Memory-Attachment ilişkisini sil
        await _attachmentRepo.RemoveAttachmentFromMemoryAsync(memoryId, attachmentId);

        // Eğer bu attachment başka hiçbir memory'de kullanılmıyorsa, B2'den de sil
        var otherUsages = await _memoryAttachmentRepo.GetAllAsync(ct);
        var isUsedElsewhere = otherUsages.Any(ma => 
            ma.AttachmentId == attachmentId && 
            ma.MemoryId != memoryId && 
            !ma.IsDeleted);

        if (!isUsedElsewhere)
        {
            // B2'den sil
            await _storageService.DeleteFileAsync(attachment.FilePath, ct);
            
            // DB'den sil
            await _attachmentRepo.DeleteAttachmentAsync(attachmentId);
        }

        return true;
    }

    /// <summary>
    /// Memory'yi siler (soft delete)
    /// </summary>
    public async Task<bool> DeleteMemoryAsync(int memoryId, int userId, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null || memory.UserId != userId)
            throw new UnauthorizedAccessException();

        await _memoryRepo.DeleteAsync(memoryId, ct);
        return true;
    }

    /// <summary>
    /// Attachment caption'ını günceller
    /// </summary>
    public async Task UpdateAttachmentCaptionAsync(
        int memoryId,
        int attachmentId,
        string caption,
        int userId,
        CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null || memory.UserId != userId)
            throw new UnauthorizedAccessException();

        var memoryAttachment = await _memoryAttachmentRepo.GetByMemoryAndAttachmentIdAsync(memoryId, attachmentId, ct);
        if (memoryAttachment == null)
            throw new Exception("Attachment not found in this memory");

        memoryAttachment.Caption = caption;
        await _memoryAttachmentRepo.UpdateAsync(memoryAttachment, ct);
    }

    /// <summary>
    /// Attachment sıralamasını günceller
    /// </summary>
    public async Task ReorderAttachmentsAsync(
        int memoryId,
        Dictionary<int, int> attachmentOrders, // attachmentId -> newDisplayOrder
        int userId,
        CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null || memory.UserId != userId)
            throw new UnauthorizedAccessException();

        foreach (var (attachmentId, newOrder) in attachmentOrders)
        {
            var memoryAttachment = await _memoryAttachmentRepo.GetByMemoryAndAttachmentIdAsync(memoryId, attachmentId, ct);
            if (memoryAttachment != null)
            {
                memoryAttachment.DisplayOrder = newOrder;
                await _memoryAttachmentRepo.UpdateAsync(memoryAttachment, ct);
            }
        }
    }

    private MemoryResponse MapToDto(Memory memory)
    {
        return new MemoryResponse
        {
            Id = memory.Id,
            Description = memory.Description,
            Date = memory.Date,
            UserId = memory.UserId,
            CreatedAt = memory.CreatedAt,
            Attachments = memory.MemoryAttachments
                .Where(ma => !ma.IsDeleted && !ma.Attachment.IsDeleted)
                .OrderBy(ma => ma.DisplayOrder)
                .Select(ma => new AttachmentResponse()
                {
                    Id = ma.Attachment.Id,
                    FileName = ma.Attachment.FileName,
                    ContentType = ma.Attachment.ContentType,
                    FileSize = ma.Attachment.FileSize,
                    FileUrl = _storageService.GetFileUrl(ma.Attachment.FilePath),
                    DisplayOrder = ma.DisplayOrder,
                    Caption = ma.Caption,
                    UploadedAt = ma.Attachment.CreatedAt
                })
                .ToList()
        };
    }    
}