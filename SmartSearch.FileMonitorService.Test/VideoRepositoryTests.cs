using Bogus;
using Microsoft.EntityFrameworkCore;
using SmartSearch.Worker.FileMonitorService.Models.Video;
using SmartSearch.Worker.FileMonitorService.Persistence;
using SmartSearch.Worker.FileMonitorService.Services.Repository;

namespace SmartSearch.FileMonitorService.Test;

[TestClass]
public class VideoRepositoryTests
{
    private ServiceDbContext _dbContext;
    private VideoRepository _repository;
    private int recordCount = 1;
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

        _repository = new VideoRepository(optionBuilder.Options);

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
    public void TestAddNewVideos()
    {
        var videos = GenerateRandomData(recordCount);
        _repository.Insert(videos);

        var entity = _dbContext.Videos
            .Include(i => i.VideoTopic)
            .Include(i => i.VideoKeywords)
            .FirstOrDefault();

        Assert.IsNotNull(entity);

        Assert.IsNotNull(entity.VideoTopic);
        Assert.IsTrue(entity.VideoTopic.Number >= topicNumberMinValue && entity.VideoTopic.Number <= topicNumberMaxValue);
        Assert.AreEqual(entity.Id, entity.VideoTopic.VideoId);

        Assert.IsNotNull(entity.VideoKeywords);
        Assert.AreEqual(recordCount, entity.VideoKeywords.Count);
        foreach (var keyword in entity.VideoKeywords)
        {
            Assert.AreEqual(entity.Id, keyword.VideoId);
            Assert.AreEqual(entity.VideoTopic.Id, keyword.TopicId);
        }
    }

    private List<Video> GenerateRandomVideoData(int count)
    {
        Random random = new Random();

        var faker = new Faker<Video>()
            .RuleFor(v => v.Name, f => $"file_{random.Next()}.pdf")
            .RuleFor(v => v.Location, f => f.System.DirectoryPath())
            .RuleFor(v => v.Language, f => f.PickRandom(new List<string> { "de", "en" }))
            .RuleFor(v => v.TopicProbabilty, f => f.Random.Decimal());

        return faker.Generate(count);
    }

    private List<VideoTopic> GenerateRandomVideoTopicData(int count)
    {
        var faker = new Faker<VideoTopic>()
            .RuleFor(vt => vt.Number, f => f.Random.Int(topicNumberMinValue, topicNumberMaxValue))
            .RuleFor(vt => vt.Video, f => new Video());

        return faker.Generate(count);
    }

    private List<VideoKeyword> GenerateRandomVideoKeywordData(int count)
    {
        var faker = new Faker<VideoKeyword>()
            .RuleFor(vk => vk.Name, f => f.Random.Hash())
            .RuleFor(vk => vk.ClipStart, f => f.Random.Decimal())
            .RuleFor(vk => vk.ClipDuration, f => f.Random.Decimal());

        return faker.Generate(count);
    }

    private List<Video> GenerateRandomData(int count = 10)
    {
        var videos = GenerateRandomVideoData(count);
        var topics = GenerateRandomVideoTopicData(count);
        var keywords = GenerateRandomVideoKeywordData(count * count);

        int keywordStartIndex = 0, keywordEndIndex = count;

        for (int i = 0; i < count; i++)
        {
            var video = videos[i];
            var topic = topics[i];

            for (int j = keywordStartIndex; j < keywordEndIndex; j++)
            {
                keywords[j].Video = video;
                keywords[j].VideoTopic = topic;
            }
            keywordStartIndex = keywordEndIndex;
            keywordEndIndex = keywordEndIndex + count;

            topic.Video = video;
            topic.VideoKeywords = keywords.GetRange(i * count, count);

            video.VideoTopic = topic;
            video.VideoKeywords = keywords.GetRange(i * count, count);
        }

        return videos;
    }
}
