using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Models.User;
using SmartSearch.Worker.FileMonitorService.Models.Video;

namespace SmartSearch.Worker.FileMonitorService.Persistence;

public sealed class ServiceDbContext : DbContext
{
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentTopic> DocumentTopics { get; set; }
    public DbSet<DocumentKeyword> DocumentKeywords { get; set; }

    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoTopic> VideoTopics { get; set; }
    public DbSet<VideoKeyword> VideoKeywords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>().Property(p => p.CreatedBy).IsRequired().HasDefaultValue("SYSTEM");
        modelBuilder.Entity<AppUser>().Property(p => p.Created).IsRequired().HasDefaultValue(DateTime.UtcNow);
        modelBuilder.Entity<AppUser>().Property(p => p.LastModifiedBy).IsRequired().HasDefaultValue("SYSTEM");
        modelBuilder.Entity<AppUser>().Property(p => p.LastModified).IsRequired().HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<Document>().HasIndex(e => e.Name).IsUnique();
        modelBuilder.Entity<DocumentTopic>().HasIndex(e => new { e.Number, e.DocumentId }).IsUnique();
        modelBuilder.Entity<DocumentKeyword>().HasIndex(e => new { e.Name, e.TopicId, e.DocumentId }).IsUnique();

        modelBuilder.Entity<Document>().Property(p => p.TopicProbabilty)
            .HasColumnType("decimal")
            .HasPrecision(20, 18)
            .IsRequired();

        modelBuilder.Entity<DocumentKeyword>()
            .HasOne<DocumentTopic>(e => e.DocumentTopic)
            .WithMany(d => d.DocumentKeywords)
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        modelBuilder.Entity<Video>().HasIndex(e => e.Name).IsUnique();
        modelBuilder.Entity<VideoTopic>().HasIndex(e => new { e.Number, e.VideoId }).IsUnique();
        modelBuilder.Entity<VideoKeyword>().HasIndex(e => new { e.Name, e.TopicId, e.VideoId, e.ClipStart }).IsUnique();

        modelBuilder.Entity<Video>().Property(p => p.TopicProbabilty)
            .HasColumnType("decimal")
            .HasPrecision(20, 18)
            .IsRequired();

        modelBuilder.Entity<VideoKeyword>().Property(p => p.ClipStart)
            .HasColumnType("decimal")
            .HasPrecision(20, 10)
            .IsRequired();

        modelBuilder.Entity<VideoKeyword>().Property(p => p.ClipDuration)
            .HasColumnType("decimal")
            .HasPrecision(20, 10)
            .IsRequired();

        modelBuilder.Entity<VideoKeyword>()
            .HasOne<VideoTopic>(e => e.VideoTopic)
            .WithMany(v => v.VideoKeywords)
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
    }
}
