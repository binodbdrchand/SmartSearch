using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Modules.UserManager.Repository;
using AppUserDomain = SmartSearch.Modules.UserManager.Domain;


namespace SmartSearch.Infrastructure.Persistence.Repository.AppUser;

public class AppUserRepository : GenericRepository<AppUserDomain.AppUser>, IAppUserRepository
{
    public AppUserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
