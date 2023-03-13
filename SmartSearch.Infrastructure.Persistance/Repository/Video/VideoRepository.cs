using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Modules.VideoManager.Repository;
using VideoDomain = SmartSearch.Modules.VideoManager.Domain;

namespace SmartSearch.Infrastructure.Persistence.Repository.Video;
public class VideoRepository : GenericRepository<VideoDomain.Video>, IVideoRepository
{
    public VideoRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
