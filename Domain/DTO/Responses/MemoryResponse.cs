namespace yeni.Domain.DTO.Responses;

public class MemoryResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AttachmentResponse> Attachments { get; set; } = new();
}