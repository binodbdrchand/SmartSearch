using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using DocumentDomain = SmartSearch.Modules.DocumentManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Document;
public class VideoTopicSchema : IEntityTypeConfiguration<DocumentDomain.DocumentTopic>
{
    public void Configure(EntityTypeBuilder<DocumentDomain.DocumentTopic> builder)
    {
        builder.ToTable(nameof(DocumentDomain.DocumentTopic));
        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Number)
            .IsRequired();

        builder.Property(dt => dt.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(dt => dt.Created)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(dt => dt.LastModifiedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(dt => dt.LastModified)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(dt => new { dt.Number, dt.DocumentId }).IsUnique();

        builder.HasOne<DocumentDomain.Document>(d => d.Document)
            .WithOne(dt => dt.DocumentTopic)
            .HasForeignKey<DocumentDomain.DocumentTopic>(dt => dt.DocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
