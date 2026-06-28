using Braavo.Core.Entities;
using Braavo.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Data;

public class BraavoDbContext : DbContext
{
    public BraavoDbContext(DbContextOptions<BraavoDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVersion> ProductVersions => Set<ProductVersion>();
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<UserStory> UserStories => Set<UserStory>();
    public DbSet<Feature> Features => Set<Feature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BraavoDbContext).Assembly);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVersionConfiguration());
        modelBuilder.ApplyConfiguration(new PersonaConfiguration());
        modelBuilder.ApplyConfiguration(new UserStoryConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureConfiguration());
    }
}
