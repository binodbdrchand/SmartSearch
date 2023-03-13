using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using VideoDomain = SmartSearch.Modules.VideoManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Video;
public class VideoSchema : IEntityTypeConfiguration<VideoDomain.Video>
{
    public void Configure(EntityTypeBuilder<VideoDomain.Video> builder)
    {
        builder.ToTable(nameof(VideoDomain.Video));
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(v => v.Location)
            .IsRequired(true)
            .HasMaxLength(255);

        builder.Property(v => v.Language)
            .IsRequired(true)
            .HasMaxLength(25);

        builder.Property(v => v.TopicProbabilty)
            .IsRequired(true)
            .HasPrecision(20, 18);

        builder.Property(v => v.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(v => v.Created)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(v => v.LastModifiedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(v => v.LastModified)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(v => v.Name).IsUnique();
    }
}
