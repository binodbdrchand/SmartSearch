using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using VideoDomain = SmartSearch.Modules.VideoManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Video;
public class VideoKeywordSchema : IEntityTypeConfiguration<VideoDomain.VideoKeyword>
{
    public void Configure(EntityTypeBuilder<VideoDomain.VideoKeyword> builder)
    {
        builder.ToTable(nameof(VideoDomain.VideoKeyword));
        builder.HasKey(vk => vk.Id);

        builder.Property(vk => vk.Name)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(vk => vk.IsInClipText)
            .IsRequired(true);

        builder.Property(vk => vk.ClipStart)
            .HasDefaultValue(0m)
            .HasPrecision(20, 10);

        builder.Property(vk => vk.ClipDuration)
            .HasDefaultValue(0m)
            .HasPrecision(20, 10);

        builder.Property(vk => vk.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(vk => vk.Created)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(vk => vk.LastModifiedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasDefaultValue(null);

        builder.Property(vk => vk.LastModified)
            .IsRequired(true)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(vk => new { vk.Name, vk.ClipStart, vk.TopicId, vk.VideoId }).IsUnique();

        builder.HasOne<VideoDomain.Video>(vk => vk.Video)
            .WithMany(v => v.VideoKeywords)
            .HasForeignKey(vk => vk.VideoId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<VideoDomain.VideoTopic>(vk => vk.VideoTopic)
            .WithMany(vt => vt.VideoKeywords)
            .HasForeignKey(vk => vk.TopicId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
