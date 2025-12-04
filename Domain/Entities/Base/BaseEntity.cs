namespace yeni.Domain.Entities.Base;

public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}