namespace yeni.Domain.DTO.Responses;

public class MemoryAttachmentResponse
{
    public int Id { get; set; }
    public int MemoryId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Caption { get; set; }
    public AttachmentResponse AttachmentResponse { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}