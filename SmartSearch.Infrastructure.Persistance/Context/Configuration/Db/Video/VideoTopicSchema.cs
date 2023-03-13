using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using VideoDomain = SmartSearch.Modules.VideoManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.Video
{
    public class VideoTopicSchema : IEntityTypeConfiguration<VideoDomain.VideoTopic>
    {
        public void Configure(EntityTypeBuilder<VideoDomain.VideoTopic> builder)
        {
            builder.ToTable(nameof(VideoDomain.VideoTopic));
            builder.HasKey(vt => vt.Id);

            builder.Property(vt => vt.Number)
                .IsRequired();

            builder.Property(vt => vt.CreatedBy)
                .IsRequired(false)
                .HasMaxLength(100)
                .HasDefaultValue(null);

            builder.Property(vt => vt.Created)
                .IsRequired(true)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(vt => vt.LastModifiedBy)
                .IsRequired(false)
                .HasMaxLength(100)
                .HasDefaultValue(null);

            builder.Property(vt => vt.LastModified)
                .IsRequired(true)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(vt => new { vt.Number, vt.VideoId }).IsUnique();

            builder.HasOne<VideoDomain.Video>(vt => vt.Video)
                .WithOne(v => v.VideoTopic)
                .HasForeignKey<VideoDomain.VideoTopic>(vt => vt.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
