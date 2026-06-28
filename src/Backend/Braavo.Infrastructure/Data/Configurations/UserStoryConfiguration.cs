using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class UserStoryConfiguration : IEntityTypeConfiguration<UserStory>
{
    public void Configure(EntityTypeBuilder<UserStory> builder)
    {
        builder.ToTable("user_stories");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.ProductId).HasColumnName("product_id");
        builder.Property(s => s.PersonaId).HasColumnName("persona_id");
        builder.Property(s => s.AsA).HasColumnName("as_a").HasMaxLength(255).IsRequired();
        builder.Property(s => s.IWant).HasColumnName("i_want").HasColumnType("text").IsRequired();
        builder.Property(s => s.SoThat).HasColumnName("so_that").HasColumnType("text").IsRequired();
        builder.Property(s => s.Priority).HasColumnName("priority").HasConversion<string>();
        builder.Property(s => s.AcceptanceCriteria).HasColumnName("acceptance_criteria").HasColumnType("text[]");
        builder.Property(s => s.SortOrder).HasColumnName("sort_order");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.PersonaId);
    }
}
