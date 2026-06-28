using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("features");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id");
        builder.Property(f => f.ProductId).HasColumnName("product_id");
        builder.Property(f => f.ParentId).HasColumnName("parent_id");
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(f => f.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(f => f.Phase).HasColumnName("phase").HasConversion<string>();
        builder.Property(f => f.Effort).HasColumnName("effort").HasConversion<string>();
        builder.Property(f => f.LinkedStoryIds).HasColumnName("linked_story_ids").HasColumnType("uuid[]");
        builder.Property(f => f.SortOrder).HasColumnName("sort_order");
        builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(f => f.ProductId);
        builder.HasIndex(f => f.ParentId);
        builder.HasIndex(f => f.Phase);
    }
}
