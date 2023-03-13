using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.DocumentManager.Repository;

namespace SmartSearch.Modules.DocumentManager.Service.Implementation
{
    public class DocumentTopicService : IDocumentTopicService
    {
        private readonly IDocumentTopicRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentTopicService(IDocumentTopicRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }
}
