using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class TimelinePhaseConfiguration : IEntityTypeConfiguration<TimelinePhase>
{
    public void Configure(EntityTypeBuilder<TimelinePhase> builder)
    {
        builder.ToTable("timeline_phases");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.DurationWeeks).HasColumnName("duration_weeks");
        builder.Property(p => p.StartDate).HasColumnName("start_date");
        builder.Property(p => p.SortOrder).HasColumnName("sort_order");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(p => p.Milestones)
            .WithOne()
            .HasForeignKey(m => m.PhaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.ProductId);
    }
}
