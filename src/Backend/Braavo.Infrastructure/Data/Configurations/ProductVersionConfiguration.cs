using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.ToTable("product_versions");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("id");
        builder.Property(v => v.ProductId).HasColumnName("product_id");
        builder.Property(v => v.VersionNumber).HasColumnName("version_number");
        builder.Property(v => v.Snapshot).HasColumnName("snapshot").HasColumnType("jsonb");
        builder.Property(v => v.Comment).HasColumnName("comment").HasMaxLength(500);
        builder.Property(v => v.CreatedBy)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("created_by");
        builder.Property(v => v.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => new { v.ProductId, v.VersionNumber }).IsUnique();
    }
}
