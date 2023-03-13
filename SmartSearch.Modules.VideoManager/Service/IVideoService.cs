using SmartSearch.Modules.VideoManager.ViewModel;

namespace SmartSearch.Modules.VideoManager.Service;
public interface IVideoService
{
    VideoViewModel? GetById(int id);
    VideoViewModel GetByIdAndKeyword(int id, string keyword);
    IEnumerable<VideoViewModel> GetByKeyword(string keyword);
}
