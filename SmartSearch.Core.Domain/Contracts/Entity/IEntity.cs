namespace SmartSearch.Core.Domain.Contracts.Entity
{
    public interface IEntity<T>
    {
        public T Id { get; set; }
    }
}
