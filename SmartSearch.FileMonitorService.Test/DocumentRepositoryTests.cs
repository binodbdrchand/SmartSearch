using Bogus;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Persistence;
using SmartSearch.Worker.FileMonitorService.Services.Repository;

namespace SmartSearch.FileMonitorService.Test;

[TestClass]
public class DocumentRepositoryTests
{
    private ServiceDbContext _dbContext;
    private DocumentRepository _repository;
    private int recordCount = 25;
    private int topicNumberMinValue = 0;
    private int topicNumberMaxValue = 20;

    [TestInitialize]
    public void Init()
    {
        var optionBuilder = new DbContextOptionsBuilder<ServiceDbContext>()
            .UseSqlServer("Server=ESS-BBC\\SQLEXPRESS;Initial Catalog=SmartSearchTest;Integrated Security=True;TrustServerCertificate=True")
            //.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging();

        _dbContext = new ServiceDbContext(optionBuilder.Options);

        _repository = new DocumentRepository(optionBuilder.Options);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Database.EnsureCreated();
        _dbContext.Database.EnsureDeleted();

        _dbContext.Dispose();
    }

    [TestMethod]
    public void TestAddNewDocuments()
    {
        var documents = GenerateRandomData(recordCount);
        _repository.Insert(documents);

        var entity = _dbContext.Documents
            .Include(i => i.DocumentTopic)
            .Include(i => i.DocumentKeywords)
            .FirstOrDefault();

        Assert.IsNotNull(entity);

        Assert.IsNotNull(entity.DocumentTopic);
        Assert.IsTrue(entity.DocumentTopic.Number >= topicNumberMinValue && entity.DocumentTopic.Number <= topicNumberMaxValue);
        Assert.AreEqual(entity.Id, entity.DocumentTopic.DocumentId);

        Assert.IsNotNull(entity.DocumentKeywords);
        Assert.AreEqual(recordCount, entity.DocumentKeywords.Count);
        foreach (var keyword in entity.DocumentKeywords)
        {
            Assert.AreEqual(entity.Id, keyword.DocumentId);
            Assert.AreEqual(entity.DocumentTopic.Id, keyword.TopicId);
        }
    }

    [TestMethod]
    public void TestUpdateDocument()
    {
        Document document = new Document
        {
            Name = "test",
            Location = "C:/test",
            Language = "en",
            TopicProbabilty = 0.90M
        };
        DocumentTopic topic = new DocumentTopic
        {
            Number = 1,
            Document = document
        };
        List<DocumentKeyword> keywords = new List<DocumentKeyword>
        {
            new DocumentKeyword
            {
                Name = "test-keyword1",
                DocumentTopic = topic,
                Document = document
            },
            new DocumentKeyword
            {
                Name = "test-keyword2",
                DocumentTopic = topic,
                Document = document
            },
        };
        document.DocumentTopic = topic;
        document.DocumentKeywords = keywords;

        _repository.Insert(new List<Document> { document });

        var existingEntity = _repository.Get(document.Name);

        Assert.IsNotNull(existingEntity);
        Assert.AreEqual(1, existingEntity.Id);
        Assert.AreEqual("test", existingEntity.Name);

        Assert.AreEqual(1, existingEntity.DocumentTopic.Number);

        Assert.AreEqual(2, existingEntity.DocumentKeywords.Count);
        var keywordList = existingEntity.DocumentKeywords.ToList();
        Assert.AreEqual("test-keyword1", keywordList[0].Name);
        Assert.AreEqual("test-keyword2", keywordList[1].Name);

        document = new Document
        {
            Name = "test",
            Location = "C:/test",
            Language = "en",
            TopicProbabilty = 0.90M
        };
        topic = new DocumentTopic
        {
            Number = 1,
            Document = document
        };
        keywords = new List<DocumentKeyword>
        {
            new DocumentKeyword
            {
                Name = "test-updated-keyword1",
                DocumentTopic = topic,
                Document = document
            },
            new DocumentKeyword
            {
                Name = "test-updated-keyword2",
                DocumentTopic = topic,
                Document = document
            },
        };
        document.DocumentTopic = topic;
        document.DocumentKeywords = keywords;

        _repository.Insert(new List<Document> { document });

        existingEntity = _repository.Get(document.Name);

        Assert.IsNotNull(existingEntity);
        Assert.AreEqual(1, existingEntity.Id);
        Assert.AreEqual("test", existingEntity.Name);

        Assert.AreEqual(1, existingEntity.DocumentTopic.Number);

        Assert.AreEqual(2, existingEntity.DocumentKeywords.Count);
        keywordList = existingEntity.DocumentKeywords.ToList();
        Assert.AreEqual("test-updated-keyword1", keywordList[0].Name);
        Assert.AreEqual("test-updated-keyword2", keywordList[1].Name);
    }

    [TestMethod]
    public void TestDeleteDocument()
    {
        Document document = new Document
        {
            Name = "test",
            Location = "C:/test",
            Language = "en",
            TopicProbabilty = 0.90M
        };

        _repository.Insert(new List<Document> { document });

        _repository.Delete(document.Name);

        var entity = _repository.Get(document.Name);

        Assert.IsNull(entity);
    }

    private List<Document> GenerateRandomDocumentData(int count)
    {
        Random random = new Random();

        var faker = new Faker<Document>()
            .RuleFor(d => d.Name, f => $"file_{random.Next()}.pdf")
            .RuleFor(d => d.Location, f => f.System.DirectoryPath())
            .RuleFor(d => d.Language, f => f.PickRandom(new List<string> { "de", "en" }))
            .RuleFor(d => d.TopicProbabilty, f => f.Random.Decimal());

        return faker.Generate(count);
    }

    private List<DocumentTopic> GenerateRandomDocumentTopicData(int count)
    {
        var faker = new Faker<DocumentTopic>()
            .RuleFor(dt => dt.Number, f => f.Random.Int(topicNumberMinValue, topicNumberMaxValue))
            .RuleFor(dt => dt.Document, f => new Document());

        return faker.Generate(count);
    }

    private List<DocumentKeyword> GenerateRandomDocumentKeywordData(int count)
    {
        var faker = new Faker<DocumentKeyword>()
            .RuleFor(dk => dk.Name, f => f.Random.Hash())
            .RuleFor(dk => dk.IsInCorpus, f => f.Random.Bool());

        return faker.Generate(count);
    }

    private List<Document> GenerateRandomData(int count = 10)
    {
        var documents = GenerateRandomDocumentData(count);
        var topics = GenerateRandomDocumentTopicData(count);
        var keywords = GenerateRandomDocumentKeywordData(count * count);

        int keywordStartIndex = 0, keywordEndIndex = count;

        for (int i = 0; i < count; i++)
        {
            var document = documents[i];
            var topic = topics[i];

            for (int j = keywordStartIndex; j < keywordEndIndex; j++)
            {
                keywords[j].Document= document;
                keywords[j].DocumentTopic = topic;
            }
            keywordStartIndex = keywordEndIndex;
            keywordEndIndex = keywordEndIndex + count;

            topic.Document = document;
            topic.DocumentKeywords = keywords.GetRange(i * count, count);

            document.DocumentTopic = topic;
            document.DocumentKeywords = keywords.GetRange(i * count, count);
        }

        return documents;
    }
}