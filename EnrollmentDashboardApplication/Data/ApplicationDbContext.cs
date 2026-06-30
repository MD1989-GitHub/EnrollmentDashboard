using EnrollmentDashboardApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentDashboardApplication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.Property(x => x.Status)
                .HasMaxLength(50);
        });
    }
}