using System.ComponentModel.DataAnnotations;

namespace yeni.Domain.Entities.Base;

public class Memory : Entity
{
    public int Id { get; set; } 
    
    [Required]
    [Display(Name = "Açıklama")]
    public string Description { get; set; }
    
    [Required]
    public DateTime Date { get; set; }    
    
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public ICollection<MemoryAttachment> MemoryAttachments { get; set; } = new List<MemoryAttachment>();

}