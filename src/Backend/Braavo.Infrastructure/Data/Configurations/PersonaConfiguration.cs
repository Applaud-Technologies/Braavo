using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("personas");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.Role).HasColumnName("role").HasMaxLength(255);
        builder.Property(p => p.TechnicalLevel).HasColumnName("technical_level").HasConversion<string>();
        builder.Property(p => p.Goals).HasColumnName("goals").HasColumnType("text[]");
        builder.Property(p => p.PainPoints).HasColumnName("pain_points").HasColumnType("text[]");
        builder.Property(p => p.Motivations).HasColumnName("motivations").HasColumnType("text[]");
        builder.Property(p => p.Quote).HasColumnName("quote").HasColumnType("text");
        builder.Property(p => p.SortOrder).HasColumnName("sort_order");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(p => p.ProductId);
    }
}
