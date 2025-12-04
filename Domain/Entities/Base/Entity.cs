namespace yeni.Domain.Entities.Base;

public class Entity : BaseEntity, IEquatable<Entity>
{
    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
    
    public static bool operator ==(Entity? first, Entity? second)
    {
        return (first is null && second is null) || (first is not null && second is not null && first.Equals(second));
    }
    
    public static bool operator !=(Entity? first, Entity? second) =>
        !(first == second);

    public bool Equals(Entity? other)
    {
        if (other is null || other.GetType() != GetType() || other.GetHashCode() != GetHashCode())
            return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType() || obj is not Entity entity || !GetHashCode().Equals(entity.GetHashCode()))
            return false;

        return true;
    }

    public override int GetHashCode() => GetHashCode() * 41;
}