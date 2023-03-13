using SmartSearch.Modules.UserManager.ViewModel;


namespace SmartSearch.Modules.UserManager.Service;

public interface IAppUserService
{
    List<AppUserViewModel> GetAll();
    AppUserViewModel? GetByEmail(string email, bool isAdmin = false);    void Insert(AppUserViewModel appUser);
    void Update(AppUserViewModel appUser);
    void Delete(string email);
}
