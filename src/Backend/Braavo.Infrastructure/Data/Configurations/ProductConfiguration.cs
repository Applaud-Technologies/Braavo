using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.OwnerId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("owner_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(p => p.Categories).HasColumnName("categories").HasColumnType("text[]");
        builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(p => p.Version).HasColumnName("version");
        builder.Property(p => p.CompletionPercentage).HasColumnName("completion_percentage");

        builder.Property(p => p.Vision).HasColumnName("vision").HasColumnType("text");
        builder.Property(p => p.ProblemStatement).HasColumnName("problem_statement").HasColumnType("text");
        builder.Property(p => p.ValueProposition).HasColumnName("value_proposition").HasColumnType("text");
        builder.Property(p => p.TargetMarket).HasColumnName("target_market").HasColumnType("text[]");
        builder.Property(p => p.BusinessGoals).HasColumnName("business_goals").HasColumnType("text[]");

        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(p => p.OwnerId);
    }
}
