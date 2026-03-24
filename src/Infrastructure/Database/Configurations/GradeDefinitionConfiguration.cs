using Domain.EducationalSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

public class GradeDefinitionConfiguration : IEntityTypeConfiguration<GradeDefinition>
{
    public void Configure(EntityTypeBuilder<GradeDefinition> builder)
    {
        builder.ToTable("grade_definitions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, v => GradeDefinitionId.From(v))
            .ValueGeneratedNever();

        builder.Property(g => g.GradeCycleDefinitionId)
            .HasColumnName("grade_cycle_definition_id")
            .HasConversion(id => id.Value, v => GradeCycleDefinitionId.From(v))
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Name_Fr)
            .HasColumnName("name_fr")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Name_Ar)
            .HasColumnName("name_ar")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order");

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);
        // Navigation back to parent cycle — read-only, used only by Include()
        // The FK is already on this table (grade_cycle_definition_id).
        // GradeCycleDefinitionConfiguration.HasMany(...).WithOne() owns
        // the relationship — here we just name the navigation property.
        builder.HasOne(g => g.Cycle)
            .WithMany(c => c.Grades)
            .HasForeignKey(g => g.GradeCycleDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
