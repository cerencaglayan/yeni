using Microsoft.EntityFrameworkCore;
using yeni.Domain.Entities.Base;

namespace yeni.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Memory> Memories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    
}