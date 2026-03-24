using Domain.EducationalSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

public class GradeCycleDefinitionConfiguration : IEntityTypeConfiguration<GradeCycleDefinition>
{
    public void Configure(EntityTypeBuilder<GradeCycleDefinition> builder)
    {
        builder.ToTable("grade_cycle_definitions");
        builder.HasKey(gc => gc.Id);
        builder.Property(gc => gc.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, v => GradeCycleDefinitionId.From(v))
            .ValueGeneratedNever();
        builder.Property(gc => gc.EducationalSystemId)
            .HasColumnName("educational_system_id")
            .HasConversion(id => id.Value, v => EducationalSystemId.From(v))
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

        builder.Property(x => x.BroadLevel)
            .HasColumnName("broad_level")
            .HasConversion<int>();

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order");

    }
}
