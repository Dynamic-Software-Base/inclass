using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasMaxLength(500)
            ;
        builder.Property(x => x.Payload)
            .HasColumnName("payload")
            .HasColumnType("text");

        builder.Property(x => x.ProcessedOn)
            .HasColumnName("processed_on")
            .IsRequired(false);
        builder.Property(x => x.Error)
            .HasColumnName("error")
            .IsRequired(false);
        builder.Property(x => x.OccurredOn)
            .HasColumnName("occurred_on")
            .IsRequired();
    }
}
