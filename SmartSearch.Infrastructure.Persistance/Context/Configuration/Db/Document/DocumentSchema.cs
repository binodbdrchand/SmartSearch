using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using DocumentDomain = SmartSearch.Modules.DocumentManager.Domain;


namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Document;

public class DocumentSchema : IEntityTypeConfiguration<DocumentDomain.Document>
{
    public void Configure(EntityTypeBuilder<DocumentDomain.Document> builder)
    {
        builder.ToTable(nameof(DocumentDomain.Document));
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(d => d.Location)
            .IsRequired(true)
            .HasMaxLength(255);

        builder.Property(d => d.Language)
            .IsRequired(true)
            .HasMaxLength(25);

        builder.Property(d => d.TopicProbabilty)
            .IsRequired(true)
            .HasPrecision(20, 18);

        builder.Property(d => d.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(d => d.Created)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(d => d.LastModifiedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(d => d.LastModified)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(d => d.Name).IsUnique();
    }
}
