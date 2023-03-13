using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SmartSearch.Core.Domain.Contracts.Entity;
using SmartSearch.Modules.DocumentManager.Domain;
using SmartSearch.Modules.UserManager.Domain;
using SmartSearch.Modules.VideoManager.Domain;

namespace SmartSearch.Infrastructure.Persistance.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentTopic> DocumentTopics { get; set; }
        public DbSet<DocumentKeyword> DocumentKeywords { get; set; }

        public DbSet<Video> Video { get; set; }
        public DbSet<VideoTopic> VideoTopics { get; set; }
        public DbSet<VideoKeyword> VideoKeywords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            new DbInitializer(modelBuilder).SeedAppUser();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //configurationBuilder..Conventions.Remove(typeof(ForeignKeyIndexConvention));
        }

        public override int SaveChanges()
        {
            // Get all the entities that inherit from AuditableEntity
            // and have a state of Added or Modified
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            // For each entity we will set the Audit properties
            foreach (var entityEntry in entries)
            {
                // If the entity state is Added let's set
                // the CreatedAt and CreatedBy properties
                if (entityEntry.State == EntityState.Added)
                {
                    ((IAuditable)entityEntry.Entity).Created = DateTime.UtcNow;
                    ((IAuditable)entityEntry.Entity).CreatedBy = "APPLICATION";
                }
                else
                {
                    // If the state is Modified then we don't want
                    // to modify the CreatedAt and CreatedBy properties
                    // so we set their state as IsModified to false
                    Entry((IAuditable)entityEntry.Entity).Property(p => p.Created).IsModified = false;
                    Entry((IAuditable)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
                }

                // In any case we always want to set the properties
                // ModifiedAt and ModifiedBy
                ((IAuditable)entityEntry.Entity).LastModified = DateTime.UtcNow;
                ((IAuditable)entityEntry.Entity).LastModifiedBy = "APPLICATION";
            }
            // After we set all the needed properties
            // we call the base implementation of SaveChanges
            // to actually save our entities in the database
            return base.SaveChanges();
        }
    }
}
