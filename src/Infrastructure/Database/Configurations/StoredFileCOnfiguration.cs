using Domain.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("stored_files");

        builder.HasKey(sf => sf.Id);

        builder.Property(sf => sf.Id)
            .HasConversion(
                storedId => storedId.Value,
                value => StoredFileId.From(value)
            )
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(sf => sf.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired()
            .HasConversion(
                ownerId => ownerId.Value,
                value => UserId.From(value)
            );
        builder.Property(f => f.OriginalFileName)
            .HasMaxLength(500)
            .HasColumnName("original_file_name")
            .IsRequired();

        builder.Property(f => f.StoredFileName)
            .HasMaxLength(500)
            .HasColumnName("stored_file_name")
            .IsRequired();

        builder.Property(f => f.ContentType)
            .HasMaxLength(100)
            .HasColumnName("content_type")
            .IsRequired();

        builder.Property(f => f.SizeInBytes)
            .HasColumnName("size_in_bytes")
            .IsRequired();
        builder.Property(f => f.StorageProvider)
            .HasMaxLength(100)
            .HasColumnName("storage_provider")
            .IsRequired();

        builder.Property(f => f.BlobPath)
            .HasMaxLength(2000)
            .HasColumnName("blob_path");
        builder.Property(f => f.Url)
            .HasColumnName("url")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(school => school.CreatedBy)
            .HasColumnName("created_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));

        builder.Property(school => school.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasConversion(
                userId => userId.Value,
                value => UserId.From(value));
        builder.Property(f => f.LastModifiedAt)
            .HasColumnName("last_modified_at");
        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");
        // Indexes
        builder.HasIndex(f => f.OwnerId);

        builder.HasIndex(f => f.StoredFileName)
            .IsUnique();
    }
}
