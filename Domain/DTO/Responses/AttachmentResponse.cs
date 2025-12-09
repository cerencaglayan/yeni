namespace yeni.Domain.DTO.Responses;

public class AttachmentResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public string FileUrl { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
}