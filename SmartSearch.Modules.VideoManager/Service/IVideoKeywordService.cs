using SmartSearch.Modules.VideoManager.ViewModel;

namespace SmartSearch.Modules.VideoManager.Service;
public interface IVideoKeywordService
{
    IEnumerable<VideoKeywordViewModel> GetAll();
    IEnumerable<VideoKeywordViewModel> GetAll(string keyword);
    IEnumerable<string> GetAllDistinct(string keyword);
    IEnumerable<VideoKeywordViewModel> GetAllIncluding();
    IEnumerable<VideoKeywordViewModel> GetAllIncluding(string keyword);
}
