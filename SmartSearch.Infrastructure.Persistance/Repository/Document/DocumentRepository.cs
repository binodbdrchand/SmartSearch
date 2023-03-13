using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Modules.DocumentManager.Repository;

using DocumentDomain = SmartSearch.Modules.DocumentManager.Domain;


namespace SmartSearch.Infrastructure.Persistence.Repository.Document;

public class DocumentRepository : GenericRepository<DocumentDomain.Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}