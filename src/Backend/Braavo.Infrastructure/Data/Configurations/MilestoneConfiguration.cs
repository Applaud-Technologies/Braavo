using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.ToTable("milestones");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.PhaseId).HasColumnName("phase_id");
        builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(m => m.WeekNumber).HasColumnName("week_number");
        builder.Property(m => m.Deliverables).HasColumnName("deliverables").HasColumnType("text[]");
        builder.Property(m => m.Status).HasColumnName("status").HasMaxLength(50);
        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(m => m.PhaseId);
    }
}
