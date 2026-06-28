using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.ProjectId).HasColumnName("project_id");
        builder.Property(d => d.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(d => d.Content).HasColumnName("content").HasColumnType("jsonb");
        builder.Property(d => d.Type).HasColumnName("type").HasConversion<string>();
        builder.Property(d => d.Version).HasColumnName("version");
        builder.Property(d => d.CreatedBy)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("created_by");
        builder.Property(d => d.CreatedAt).HasColumnName("created_at");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(d => d.ProjectId);
    }
}
