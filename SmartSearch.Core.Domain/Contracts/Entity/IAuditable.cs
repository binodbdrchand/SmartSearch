namespace SmartSearch.Core.Domain.Contracts.Entity
{
    public interface IAuditable
    {
        string? CreatedBy { get; set; }
        DateTime Created { get; set; }
        string? LastModifiedBy { get; set; }
        DateTime LastModified { get; set; }
    }
}