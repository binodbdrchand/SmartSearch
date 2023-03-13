using SmartSearch.Core.Domain.Contracts.Entity;

namespace SmartSearch.Core.Domain.Entity.Base
{
    public class EntityBase <T> : IEntity<T>, IAuditable
    {
        public virtual T Id { get; set; }
        public virtual string? CreatedBy { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string? LastModifiedBy { get; set; }
        public virtual DateTime LastModified { get; set; }
    }
}
