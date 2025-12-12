using Microsoft.EntityFrameworkCore;
using yeni.Domain.Entities;
using yeni.Domain.Entities.Base;

namespace yeni.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Memory> Memories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<MemoryAttachment> MemoryAttachments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>();
        modelBuilder.Entity<Memory>();
        modelBuilder.Entity<RefreshToken>();
        modelBuilder.Entity<Attachment>();
        modelBuilder.Entity<MemoryAttachment>();
    }
}