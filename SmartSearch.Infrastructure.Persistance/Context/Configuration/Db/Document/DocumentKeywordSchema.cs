using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using DocumentDomain = SmartSearch.Modules.DocumentManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Document;
public class DocumentKeywordSchema : IEntityTypeConfiguration<DocumentDomain.DocumentKeyword>
{
    public void Configure(EntityTypeBuilder<DocumentDomain.DocumentKeyword> builder)
    {
        builder.ToTable(nameof(DocumentDomain.DocumentKeyword));
        builder.HasKey(dk => dk.Id);

        builder.Property(dk => dk.Name)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(dk => dk.IsInCorpus)
            .IsRequired(true);

        builder.Property(dk => dk.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(dk => dk.Created)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(dk => dk.LastModifiedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(dk => dk.LastModified)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(dk => new { dk.Name, dk.TopicId, dk.DocumentId }).IsUnique();

        builder.HasOne<DocumentDomain.Document>(dk => dk.Document)
            .WithMany(d => d.DocumentKeywords)
            .HasForeignKey(dk => dk.DocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<DocumentDomain.DocumentTopic>(dk => dk.DocumentTopic)
            .WithMany(dt => dt.DocumentKeywords)
            .HasForeignKey(dk => dk.TopicId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
