using SmartSearch.Modules.DocumentManager.ViewModel;

namespace SmartSearch.Modules.DocumentManager.Service
{
    public interface IDocumentKeywordService
    {
        IEnumerable<DocumentKeywordViewModel> GetAll();
        IEnumerable<DocumentKeywordViewModel> GetAll(string keyword);
        IEnumerable<string> GetAllDistinct(string keyword);
        IEnumerable<DocumentKeywordViewModel> GetAllIncluding();
        IEnumerable<DocumentKeywordViewModel> GetAllIncluding(string keyword);
    }
}
