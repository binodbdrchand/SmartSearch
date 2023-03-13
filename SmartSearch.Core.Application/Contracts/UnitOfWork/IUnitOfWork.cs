namespace SmartSearch.Core.Application.Contracts.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
