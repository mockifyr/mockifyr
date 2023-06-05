using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Configuration> Configurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Codes for creating models and defining relationships
    }
}
