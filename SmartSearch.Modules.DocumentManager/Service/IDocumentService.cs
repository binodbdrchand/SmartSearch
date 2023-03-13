using SmartSearch.Modules.DocumentManager.ViewModel;


namespace SmartSearch.Modules.DocumentManager.Service;

public interface IDocumentService
{
    DocumentViewModel? GetById(int id);
}
