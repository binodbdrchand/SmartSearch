using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Modules.DocumentManager.Domain;
using SmartSearch.Modules.DocumentManager.Repository;

namespace SmartSearch.Infrastructure.Persistence.Repository.Document
{
    public class DocumentTopicRepository : GenericRepository<DocumentTopic>, IDocumentTopicRepository
    {
        public DocumentTopicRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
