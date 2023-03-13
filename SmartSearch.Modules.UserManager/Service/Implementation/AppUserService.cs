using AutoMapper;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Core.Domain.Exceptions;
using SmartSearch.Modules.UserManager.Domain;
using SmartSearch.Modules.UserManager.Repository;
using SmartSearch.Modules.UserManager.ViewModel;
using System;

namespace SmartSearch.Modules.UserManager.Service.Implementation;

public class AppUserService : IAppUserService
{
    private IAppUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AppUserService(IAppUserRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public List<AppUserViewModel> GetAll()
    {
        var result = _repository.GetAll();

        return _mapper.Map<List<AppUserViewModel>>(result);
    }

    public AppUserViewModel? GetByEmail(string email, bool isAdmin = false)
    {
        AppUser? appUser;

        if (isAdmin)
        {
            appUser = _repository.GetQueryable()
                .Where(x => x.Email == email)
                .FirstOrDefault();
        }
        else
        {
            appUser = _repository.GetQueryable()
            .Where(x => x.IsActive && x.Email == email)
            .FirstOrDefault();
        }

        return _mapper.Map<AppUserViewModel>(appUser);
    }

    public void Insert(AppUserViewModel appUser)
    {
        var user = _mapper.Map<AppUser>(appUser);

        _repository.Add(user);
        _unitOfWork.SaveChanges();
    }

    public void Update(AppUserViewModel appUser)
    {
        var user = _mapper.Map<AppUser>(appUser);

        _repository.Update(user);
        _unitOfWork.SaveChanges();
    }

    public void Delete(string email)
    {
        var user = _repository.GetQueryable()
            .Where(x => x.Email == email)
            .FirstOrDefault();

        _repository.Remove(user);
        _unitOfWork.SaveChanges();
    }
}
