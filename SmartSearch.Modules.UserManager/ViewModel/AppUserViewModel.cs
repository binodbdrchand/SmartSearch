using SmartSearch.Modules.UserManager.Domain;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartSearch.Modules.UserManager.ViewModel;

public class AppUserViewModel
{
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
    public string DepartmentDescription
    {
        get
        {
            var description = (Department)Department;
            return description.ToString();
        }
    }

    public Role Role { get; set; }

    public String RoleDescription
    {
        get
        {
            var description = (Role)Role;
            return description.ToString();
        }
    }

    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; }
}