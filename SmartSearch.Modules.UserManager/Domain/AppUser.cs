using SmartSearch.Core.Domain.Entity.Base;


namespace SmartSearch.Modules.UserManager.Domain;

public class AppUser : EntityBase<int>
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Department Department { get; set; }
    public Role Role { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
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