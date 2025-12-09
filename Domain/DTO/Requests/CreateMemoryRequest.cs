namespace yeni.Domain.DTO.Requests;

public class CreateMemoryRequest
{
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public List<IFormFile>? Files { get; set; }
    public List<string?>? Captions { get; set; }    
}