using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.VideoManager.Repository;
using SmartSearch.Modules.VideoManager.ViewModel;

namespace SmartSearch.Modules.VideoManager.Service.Implementation;
public class VideoKeywordService : IVideoKeywordService
{
    private readonly IVideoKeywordRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VideoKeywordService(IVideoKeywordRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public IEnumerable<VideoKeywordViewModel> GetAll()
    {
        var result = _repository.GetAll();

        return _mapper.Map<IEnumerable<VideoKeywordViewModel>>(result);
    }

    public IEnumerable<VideoKeywordViewModel> GetAll(string keyword)
    {
        var result = _repository.Get(x => x.Name == keyword)
            .ToList();

        return _mapper.Map<IEnumerable<VideoKeywordViewModel>>(result);
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

    public IEnumerable<VideoKeywordViewModel> GetAllIncluding()
    {
        var result = _repository.GetQueryable()
            .Include(i => i.VideoTopic)
            .Include(i => i.Video)
            .ToList();

        return _mapper.Map<IEnumerable<VideoKeywordViewModel>>(result);
    }

    public IEnumerable<VideoKeywordViewModel> GetAllIncluding(string keyword)
    {
        var result = _repository.GetQueryable()
            .Where(x => x.Name == keyword)
            .Include(i => i.VideoTopic)
            .Include(i => i.Video)
            .ToList();

        return _mapper.Map<IEnumerable<VideoKeywordViewModel>>(result);
    }
}
