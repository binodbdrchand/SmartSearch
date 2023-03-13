using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSearch.Modules.UserManager.Domain;
using AppUserDomain = SmartSearch.Modules.UserManager.Domain;


namespace SmartSearch.Infrastructure.Persistence.Context.Configuration.Db.AppUser;

public class AppUserSchema : IEntityTypeConfiguration<AppUserDomain.AppUser>
{
    public void Configure(EntityTypeBuilder<AppUserDomain.AppUser> builder)
    {
        builder.ToTable(nameof(AppUserDomain.AppUser));
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FullName)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(d => d.Email)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(d => d.Password)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(d => d.Department)
            .IsRequired(true)
            .HasDefaultValue(Department.NA);

        builder.Property(d => d.Role)
            .IsRequired(true)
            .HasDefaultValue(Role.User);

        builder.Property(d => d.IsAdmin)
            .IsRequired(true)
            .HasDefaultValue(false);

        builder.Property(d => d.IsActive)
            .IsRequired(true)
            .HasDefaultValue(true);

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

        builder.HasIndex(d => d.Email).IsUnique();
    }
}
