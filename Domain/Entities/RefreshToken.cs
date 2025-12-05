namespace yeni.Domain.Entities.Base;

public class RefreshToken : Entity
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    
    public User? User { get; set; }
    
    private RefreshToken(Guid id, string token, DateTime createdAt, DateTime expiresAt, bool isRevoked, int userId)
    {
        Id = id;
        Token = token;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = isRevoked;
        UserId = userId;
    }

    public static RefreshToken Create(string token, DateTime expiresAt, int userId)
    {
        return new RefreshToken(Guid.NewGuid(), token, DateTime.UtcNow, expiresAt, false, userId);
    }

    
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }
}