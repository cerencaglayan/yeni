using yeni.Domain.Repository;

namespace yeni.Domain.Entities.Base;

public class Attachment : Entity
{
    public int Id { get; set; }
    
    public string FileName { get; set; } = null!;
    
    public string FilePath { get; set; } = null!;
    
    public string ContentType { get; set; } = null!;
    
    public long FileSize { get; set; }
    
    public int UploadedByUserId { get; set; }
    
    public User UploadedByUser { get; set; } = null!;
    
    // Many-to-many relationship
    public ICollection<MemoryAttachment> MemoryAttachments { get; set; } = new List<MemoryAttachment>();    
    
    private Attachment(int id, string fileName, string filePath, string contentType, long fileSize, int uploadedByUserId)
    {
        Id = id;
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        UploadedByUserId = uploadedByUserId;
    }

    public Attachment() { }

    public static Attachment Create(string fileName, string filePath, string contentType, long fileSize, int uploadedByUserId)
    {
        return new Attachment(0, fileName, filePath, contentType, fileSize, uploadedByUserId);
    }
    
}