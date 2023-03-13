using Microsoft.EntityFrameworkCore;
using SmartSearch.Modules.UserManager.Domain;


namespace SmartSearch.Infrastructure.Persistance.Context;

public class DbInitializer
{
    private readonly ModelBuilder _modelBuilder;

    public DbInitializer(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    public void SeedAppUser()
    {
        _modelBuilder.Entity<AppUser>()
            .HasData(
                new AppUser
                {
                    Id = 1,
                    FullName = "Administrator",
                    Email = "admin@admin.org",
                    Password = PasswordEncoder("admin"),
                    Department = Department.NA,
                    Role = Role.Admin,
                    IsAdmin = true,
                    IsActive = true,
                    CreatedBy = "SYSTEM",
                    LastModifiedBy = "SYSTEM"
                }
            );
    }

    public string PasswordEncoder(string password)
    {
        byte[] passwordBytes = new byte[password.Length];
        passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        
        return Convert.ToBase64String(passwordBytes);
    }
}
