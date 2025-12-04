namespace yeni.Domain.Entities.Base;

public class User : Entity
{
    public int Id { get; set; }  
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string? Email { get; set; }
    
    public string? Password { get; set; }
    
    public DateTime? Birthday { get; set; }

}