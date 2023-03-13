using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.DocumentManager.Repository;
using SmartSearch.Modules.DocumentManager.ViewModel;


namespace SmartSearch.Modules.DocumentManager.Service.Implementation;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public DocumentViewModel? GetById(int id)
    {
        var result = _repository.GetById(id);

        return _mapper.Map<DocumentViewModel>(result);
    }
}
