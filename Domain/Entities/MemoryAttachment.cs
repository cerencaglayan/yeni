namespace yeni.Domain.Entities.Base;

public class MemoryAttachment : Entity
{
    public int Id { get; set; }
    
    public int MemoryId { get; set; }
    
    public int AttachmentId { get; set; }
    
    public int DisplayOrder { get; set; }  // Fotoğrafların sırasını tutmak için
    
    public string? Caption { get; set; }  // Her memory'de farklı açıklama olabilir
    
    // Navigation properties
    public Memory Memory { get; set; } = null!;
    
    public Attachment Attachment { get; set; } = null!;

    private MemoryAttachment(int id, int memoryId, int AttachmentId, int displayOrder, string? caption)
    {
        Id = id;
        MemoryId = memoryId;
        AttachmentId = AttachmentId;
        DisplayOrder = displayOrder;
        Caption = caption;
    }

    public MemoryAttachment() { }

    public static MemoryAttachment Create(int memoryId, int AttachmentId, int displayOrder, string? caption = null)
    {
        return new MemoryAttachment(0, memoryId, AttachmentId, displayOrder, caption);
    }   
}