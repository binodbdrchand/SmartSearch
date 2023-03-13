using System.Linq.Expressions;

namespace SmartSearch.Core.Application.Contracts.Repository.Base
{
    public interface IGenericRepository<T> where T : class
    {
        // ReadOnly Methods
        T? GetById(int id);
        IReadOnlyList<T> GetAll();
        IEnumerable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");
        IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        // CRUD Methods
        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}
