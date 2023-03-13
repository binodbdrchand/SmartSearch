using AutoMapper;
using Bogus;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.UnitOfWork;
using SmartSearch.Modules.DocumentManager.ViewModel;
using SmartSearch.Worker.FileMonitorService.Models.Document;

namespace SmartSearch.Modules.DocumentManager.Test
{
    [TestClass]
    public class DocumentKeywordServiceTests
    {
        ApplicationDbContext _dbContext;
        UnitOfWork _unitOfWork;
        IMapper _mapper;

        int dataCount = 5,
            topicNumberMinValue = 1,
            topicNumberMaxValue = 20;

        [TestInitialize]
        public void Init()
        {
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=ESS-BBC\\SQLEXPRESS;Initial Catalog=SmartSearchTest;Integrated Security=True;TrustServerCertificate=True")
                .EnableSensitiveDataLogging();

            _dbContext = new ApplicationDbContext(optionBuilder.Options);
            _unitOfWork = new UnitOfWork(_dbContext);

            var config = new MapperConfiguration(opts =>
            {
                opts.CreateMap<DocumentKeyword, DocumentKeywordViewModel>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_dbContext.Database.EnsureCreated();
            //_dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public void TestAllDocumentsRetrievedByTopic()
        {
            var documents = GenerateRandomData(5);
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
                .RuleFor(dt => dt.Number, f => f.Random.Int(topicNumberMinValue, topicNumberMaxValue));

            return faker.Generate(count);
        }

        private List<DocumentKeyword> GenerateRandomDocumentKeywordData(int count)
        {
            var faker = new Faker<DocumentKeyword>()
                .RuleFor(dk => dk.Name, f => f.Random.Word())
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
                    keywords[j].Document = document;
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
}