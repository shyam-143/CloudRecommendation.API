using CloudRecommendationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudRecommendation.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.User)
            .WithMany(u => u.Requirements)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recommendation>()
            .HasOne(rec => rec.Requirement)
            .WithMany(r => r.Recommendations)
            .HasForeignKey(rec => rec.RequirementId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Requirement>()
            .Property(r => r.MonthlyBudget)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Recommendation>()
            .Property(r => r.EstimatedMonthlyCost)
            .HasColumnType("decimal(18,2)");

        // Helpful indexes for performance (SQL <500ms target)
        modelBuilder.Entity<Requirement>().HasIndex(r => r.UserId);
        modelBuilder.Entity<Requirement>().HasIndex(r => r.CreatedAt);
        modelBuilder.Entity<AuditLog>().HasIndex(a => a.Timestamp);
    }
}
