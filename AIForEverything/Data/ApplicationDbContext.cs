using Microsoft.EntityFrameworkCore;
using AIForEverything.Models;

namespace AIForEverything.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ChatHistory> ChatHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<ChatHistory>()
            .HasOne(ch => ch.User)
            .WithMany(u => u.ChatHistories)
            .HasForeignKey(ch => ch.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 