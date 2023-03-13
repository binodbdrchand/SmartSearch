using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SmartSearch.Worker.FileMonitorService.Models.User;

[Table("AppUser")]
public class AppUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(50, MinimumLength = 3)]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText(true)]
    [StringLength(50, MinimumLength = 5)]
    public string Password { get; set; }

    public Department Department { get; set; }

    public Role Role { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime LastModified { get; set; }
}

public enum Department
{
    NA = 0,
    Management = 1,
    HumanResources = 2,
    Sales = 3,
    Engineering = 4
}

public enum Role
{
    SuperAdmin = 1,
    Admin = 2,
    User = 3
}