using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.DocumentManager.Repository;
using SmartSearch.Modules.DocumentManager.ViewModel;

namespace SmartSearch.Modules.DocumentManager.Service.Implementation
{
    public class DocumentKeywordService : IDocumentKeywordService
    {
        private readonly IDocumentKeywordRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentKeywordService(IDocumentKeywordRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<DocumentKeywordViewModel> GetAll()
        {
            var result = _repository.GetAll();

            return _mapper.Map<IEnumerable<DocumentKeywordViewModel>>(result);
        }

        public IEnumerable<DocumentKeywordViewModel> GetAll(string keyword)
        {
            var result = _repository.Get(x => x.Name == keyword)
                .ToList();

            return _mapper.Map<IEnumerable<DocumentKeywordViewModel>>(result);
        }

        public IEnumerable<string> GetAllDistinct(string keyword)
        {
            var result = _repository.GetQueryable()
                .Where(x => x.Name.Contains(keyword))
                .OrderBy(o => o.Name)
                .Select(s => s.Name)
                .Distinct()
                .ToList();

            return result;
        }

        public IEnumerable<DocumentKeywordViewModel> GetAllIncluding()
        {
            var result = _repository.GetQueryable()
                .Include(i => i.DocumentTopic)
                .Include(i => i.Document)
                .ToList();

            return _mapper.Map<IEnumerable<DocumentKeywordViewModel>>(result);
        }

        public IEnumerable<DocumentKeywordViewModel> GetAllIncluding(string keyword)
        {
            var result = _repository.GetQueryable()
                .Where(x => x.Name == keyword)
                .Include(i => i.DocumentTopic)
                .Include(i => i.Document)
                .ToList();

            return _mapper.Map<IEnumerable<DocumentKeywordViewModel>>(result);
        }
    }
}
