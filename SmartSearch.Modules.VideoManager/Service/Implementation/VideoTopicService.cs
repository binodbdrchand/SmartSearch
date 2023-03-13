using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Modules.VideoManager.Repository;

namespace SmartSearch.Modules.VideoManager.Service.Implementation
{
    public class VideoTopicService : IVideoTopicService
    {
        private readonly IVideoTopicRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VideoTopicService(IVideoTopicRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }
}
