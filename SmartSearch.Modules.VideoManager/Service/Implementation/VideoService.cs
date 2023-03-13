using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.VideoManager.Repository;
using SmartSearch.Modules.VideoManager.ViewModel;

namespace SmartSearch.Modules.VideoManager.Service.Implementation;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VideoService(IVideoRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public VideoViewModel? GetById(int id)
    {
        var result = _repository.GetById(id);

        return _mapper.Map<VideoViewModel>(result);
    }

    public VideoViewModel GetByIdAndKeyword(int id, string keyword)
    {
        var result = _repository.GetQueryable()
            .Where(x => x.Id == id)
            .Include(i => i.VideoTopic)
            .Include(i => i.VideoKeywords.Where(y => y.Name == keyword && y.IsInClipText).OrderBy(o => o.ClipStart))
            .FirstOrDefault();

        return _mapper.Map<VideoViewModel>(result);
    }

    public IEnumerable<VideoViewModel> GetByKeyword(string keyword)
    {
        var result = _repository.GetQueryable()
            .Include(i => i.VideoTopic)
            .Include(i => i.VideoKeywords.Where(x => x.Name == keyword))
            .OrderBy(o => o.TopicProbabilty)
            .ToList();

        return _mapper.Map<IEnumerable<VideoViewModel>>(result);
    }
}
