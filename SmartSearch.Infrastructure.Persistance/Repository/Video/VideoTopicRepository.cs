using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Modules.VideoManager.Domain;
using SmartSearch.Modules.VideoManager.Repository;

namespace SmartSearch.Infrastructure.Persistence.Repository.Video;
public class VideoTopicRepository : GenericRepository<VideoTopic>, IVideoTopicRepository
{
    public VideoTopicRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
