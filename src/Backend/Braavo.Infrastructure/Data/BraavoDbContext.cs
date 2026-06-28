using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Data;

public class BraavoDbContext : DbContext
{
    public BraavoDbContext(DbContextOptions<BraavoDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BraavoDbContext).Assembly);
    }
}
