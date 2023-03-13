using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Persistence;

namespace SmartSearch.Worker.FileMonitorService.Services.Repository;

public class DocumentRepository
{
    private readonly DbContextOptions<ServiceDbContext> _options;

    public DocumentRepository(DbContextOptions<ServiceDbContext> options)
    {
        _options = options;
    }

    public Document? Get(string name)
    {
        using var dbContext = new ServiceDbContext(_options);

        return dbContext.Documents
            .Where(x => x.Name == name)
            .Include(i => i.DocumentTopic)
            .Include(i => i.DocumentKeywords)
            .FirstOrDefault();
    }

    public async Task<Document?> GetAsync(string name)
    {
        using var dbContext = new ServiceDbContext(_options);

        return await dbContext.Documents
            .Where(x => x.Name == name)
            .Include(i => i.DocumentTopic)
            .Include(i => i.DocumentKeywords)
            .FirstOrDefaultAsync();
    }

    public void Insert(List<Document> documents)
    {
        if (documents.Count > 0)
        {
            using var dbContext = new ServiceDbContext(_options);

            foreach (var document in documents)
            {
                bool inserted = false, updated = false;

                try
                {
                    var existingEntity = dbContext.Documents
                        .Where(x => x.Name == document.Name)
                        .Include(i => i.DocumentTopic)
                        .Include(i => i.DocumentKeywords)
                        .FirstOrDefault();

                    if (existingEntity == null)
                    {
                        dbContext.Documents.Add(document);

                        inserted = true;
                        updated = false;
                    }
                    else
                    {
                        if (document.DocumentTopic != null)
                        {
                            existingEntity.DocumentTopic = document.DocumentTopic;
                            existingEntity.DocumentTopic.Document = existingEntity;
                        }

                        if (document.DocumentKeywords != null && document.DocumentKeywords.Count > 0)
                        {
                            existingEntity.DocumentKeywords = document.DocumentKeywords;

                            foreach (var keyword in existingEntity.DocumentKeywords)
                            {
                                keyword.Document = existingEntity;
                            }
                        }

                        dbContext.Update(existingEntity);

                        inserted = false;
                        updated = true;
                    }

                    dbContext.SaveChanges();

                    if (inserted)
                    {
                        Log.Information($"(DocumentRepository::Insert) -- Record Inserted -- Id: {document.Id}, Name: {document.Name}");
                    }
                    if (updated)
                    {
                        Log.Information($"(DocumentRepository::Insert) -- Record Updated -- Id: {existingEntity?.Id}, Name: {existingEntity?.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"(DocumentRepository) -- INSERT Error -- {ex}");

                    continue;
                }
            }
        }
    }

    public void InsertRange(List<Document> documents)
    {
        if (documents.Count > 0)
        {
            using var dbContext = new ServiceDbContext(_options);

            try
            {
                dbContext.AddRange(documents);
                dbContext.SaveChanges();

                documents.ForEach(d => Log.Information($"(DocumentRepository::InsertRange) -- Record Inserted -- Id: {d.Id}, Name: {d.Name}"));
            }
            catch (Exception ex)
            {
                Log.Error($"(DocumentRepository::InsertRange) -- INSERT Error -- {ex}");
            }
        }
    }

    public async void InsertRangeAsync(List<Document> documents)
    {
        if (documents.Count > 0)
        {
            try
            {
                using var dbContext = new ServiceDbContext(_options);

                await dbContext.AddRangeAsync(documents);
                await dbContext.SaveChangesAsync();

                documents.ForEach(d => Log.Information($"(DocumentRepository::InsertRangeAsync) -- Record Inserted -- Id: {d.Id}, Name: {d.Name}"));
            }
            catch (Exception ex)
            {
                Log.Error($"(DocumentRepository::InsertRangeAsync) -- INSERT Error -- {ex}");
            }
        }
    }

    public void Delete(string name)
    {
        try
        {
            using var dbContext = new ServiceDbContext(_options);

            var entity = dbContext.Documents.Where(x => x.Name == name).FirstOrDefault();
            if (entity != null)
            {
                dbContext.Documents.Remove(entity);

                dbContext.SaveChanges();

                Log.Information($"(DocumentRepository::Delete) -- Record Deleted -- Id: {entity.Id}, Name: {entity.Name}");
            }
            else 
            {
                Log.Warning($"(DocumentRepository::Delete) -- DELETE Error -- Record Not Found -- Name: {name}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(DocumentRepository::Delete) -- DELETE Error -- {ex}");
        }
    }
}
