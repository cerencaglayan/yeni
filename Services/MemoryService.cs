using yeni.Domain.Common;
using yeni.Domain.DTO.Requests;
using yeni.Domain.DTO.Responses;
using yeni.Domain.Entities.Base;
using yeni.Domain.Error;
using yeni.Domain.Repository;

namespace yeni.Configuration;

public class MemoryService(IMemoryRepository _memoryRepo,IAttachmentRepository _attachmentRepo,IMemoryAttachmentRepository _memoryAttachmentRepo,B2StorageService _storageService)
{

    /// <summary>
    /// Yeni memory oluşturur ve dosyaları yükler
    /// </summary>
    public async Task<Result<MemoryResponse>> CreateMemoryWithAttachmentsAsync(CreateMemoryRequest request, int userId, CancellationToken ct = default)
    {
        var memory = new Memory
        {
            Description = request.Description,
            Date = request.Date,
            UserId = userId
        };

        memory = await _memoryRepo.CreateAsync(memory, ct);

        var attachments = new List<Attachment>();
        if (request.Files != null && request.Files.Any())
        {
            var displayOrder = 0;
            foreach (var file in request.Files)
            {
                var filePath = await _storageService.UploadFileAsync(file, userId, ct);

                var attachment = Attachment.Create(
                    fileName: file.FileName,
                    filePath: filePath,
                    contentType: file.ContentType,
                    fileSize: file.Length,
                    uploadedByUserId: userId
                );

                attachment = await _attachmentRepo.CreateAsync(attachment, ct);
                attachments.Add(attachment);

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
    public async Task<Result<MemoryResponse>> GetMemoryByIdAsync(int memoryId, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdWithAttachmentsAsync(memoryId, ct);
        
        if (memory == null)
            return Result<MemoryResponse>.Failure(MemoryErrors.MemoryNotfound(memoryId));

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
    public async Task<Result<MemoryResponse>> AddAttachmentsToMemoryAsync(int memoryId, List<IFormFile> files, int userId, List<string?>? captions = null, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        if (memory == null)
            throw new Exception("Memory not found");

        if (memory.UserId != userId)
            throw new UnauthorizedAccessException("You can only add attachments to your own memories");

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
    public async Task<bool> RemoveAttachmentFromMemoryAsync(int memoryId, int attachmentId, int userId, CancellationToken ct = default)
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
    public async Task<Result<bool>> DeleteMemoryAsync(int memoryId, int userId, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        
        if (memory == null )
            return Result<bool>.Failure(MemoryErrors.MemoryNotfound(memoryId));

        if (memory.UserId != userId)
        {
            return Result<bool>.Failure(UserErrors.Unauthorized(userId));
        }

        await _memoryRepo.DeleteAsync(memoryId, ct);
        return true;
    }

    /// <summary>
    /// Attachment caption'ını günceller
    /// </summary>
    public async Task<Result<bool>> UpdateAttachmentCaptionAsync(int memoryId, int attachmentId, string caption, int userId, CancellationToken ct = default)
    {
        var memory = await _memoryRepo.GetByIdAsync(memoryId, ct);
        
        if (memory == null )
            return Result<bool>.Failure(MemoryErrors.MemoryNotfound(memoryId));

        if (memory.UserId != userId)
        {
            return Result<bool>.Failure(UserErrors.Unauthorized(userId));
        }

        var memoryAttachment = await _memoryAttachmentRepo.GetByMemoryAndAttachmentIdAsync(memoryId, attachmentId, ct);
        if (memoryAttachment == null)
            return Result<bool>.Failure(MemoryAttachmentErrors.MemoryAttachmentNotFound(memoryId));

        memoryAttachment.Caption = caption;
        await _memoryAttachmentRepo.UpdateAsync(memoryAttachment, ct);
        return true;
    }

    /*
     *
     * TODO: BU OLMALI MI ??? 
     */
    /// <summary>
    /// Attachment sıralamasını günceller
    /// </summary>
    public async Task ReorderAttachmentsAsync(int memoryId, Dictionary<int, int> attachmentOrders, int userId, CancellationToken ct = default)
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