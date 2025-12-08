namespace yeni.Domain.Entities.Base;

public class Memory : Entity
{
    public int Id { get; set; } 
    
    public string Description { get; set; }
    
    public DateTime Date { get; set; }    
    
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public ICollection<MemoryAttachment> MemoryAttachments { get; set; } = new List<MemoryAttachment>();

}