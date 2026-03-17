using Domain.EducationalSystem;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

public class EducationSystemConfiguration : IEntityTypeConfiguration<EducationalSystem>
{
    public void Configure(EntityTypeBuilder<EducationalSystem> builder)
    {
        builder.ToTable("educational_systems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, v => EducationalSystemId.From(v))
            .ValueGeneratedNever();

        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name_Fr).HasColumnName("name_fr").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Name_Ar).HasColumnName("name_ar").HasMaxLength(128).IsRequired();

        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasMany(e => e.Cycles)
            .WithOne()
            .HasForeignKey(e => e.EducationalSystemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<School>()              // no navigation on EducationalSystem side either
            .WithOne()                         // no navigation on School side
            .HasForeignKey(s => s.EducationalSystemId)
            .OnDelete(DeleteBehavior.Restrict); // Restrict — never delete a system that has school
    }
}
