using Bogus;
using Newtonsoft.Json;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Services;
using System.Text;

namespace SmartSearch.FileMonitorService.Test
{
    [TestClass]
    public class ModelJsonReaderTests
    {
        private StreamReader _reader;
        private int dataCount = 5;
        private int topicNumberMinValue = 0;
        private int topicNumberMaxValue = 20;

        [TestInitialize]
        public void Init()
        {
            var data = GenerateJsonData(dataCount);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));

            _reader = new StreamReader(memoryStream);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _reader.Dispose();
        }

        [TestMethod]
        public void TestParseDocumentJsonStream()
        {
            var documents = ModelJsonParser.ParseDocumentJsonStream(_reader);

            Assert.IsNotNull(documents);
            Assert.IsTrue(documents.Count == dataCount);

            var topics = documents.Select(d => d.DocumentTopic).ToList();
            Assert.IsNotNull(topics);
            Assert.IsTrue(topics.Count == dataCount);
            foreach (var topic in topics)
            {
                Assert.IsTrue(topic.Number >= topicNumberMinValue && topic.Number <= topicNumberMaxValue);
            }

            var keywords = documents.SelectMany(d => d.DocumentKeywords).ToList();
            Assert.IsNotNull(keywords);
        }

        [TestMethod]
        public void TestParseDocumentJsonString()
        {
            var documents = new List<Document>();

            for (int i = 0; i < dataCount; i++)
            {
                documents.Add(ModelJsonParser.ParseDocumentJsonString(GenerateJsonData(1)));
            }

            Assert.IsNotNull(documents);
            Assert.IsTrue(documents.Count == dataCount);

            var topics = documents.Select(d => d.DocumentTopic).ToList();
            Assert.IsNotNull(topics);
            Assert.IsTrue(topics.Count == dataCount);
            foreach (var topic in topics)
            {
                Assert.IsTrue(topic.Number >= topicNumberMinValue && topic.Number <= topicNumberMaxValue);
            }

            var keywords = documents.SelectMany(d => d.DocumentKeywords).ToList();
            Assert.IsNotNull(keywords);
        }

        private string GenerateJsonData(int count)
        {
            var filename = new Faker().System.FileName("pdf");
            var language = new List<string> { "de", "en" };

            var faker = new Faker<JsonNode>()
                .RuleFor(d => d.DocumentName, filename)
                .RuleFor(d => d.DocumentLocation, f => $"{f.System.DirectoryPath()}/{filename}")
                .RuleFor(d => d.Language, f => f.PickRandom(language))
                .RuleFor(d => d.TopicNumber, f => f.Random.Int(0, 20))
                .RuleFor(d => d.TopicProbability, f => f.Random.Decimal())
                .RuleFor(d => d.Keywords, f => string.Join(',', f.Random.WordsArray(5)))
                .RuleFor(d => d.Corpus, f => string.Join(',', f.Random.WordsArray(5)));

            var response = faker.Generate(count);

            return JsonConvert.SerializeObject(response);
        }

        private class JsonNode
        {
            public string DocumentName { get; set; }
            public string DocumentLocation { get; set; }
            public string Language { get; set; }
            public int TopicNumber { get; set; }
            public decimal TopicProbability { get; set; }
            public string Keywords { get; set; }
            public string Corpus { get; set; }

        }
    }
}
