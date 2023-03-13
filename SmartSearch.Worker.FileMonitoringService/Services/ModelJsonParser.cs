using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Models.Video;
using System.Text.RegularExpressions;
using System.Linq;

namespace SmartSearch.Worker.FileMonitorService.Services;

public static class ModelJsonParser
{
    public static List<Document> ParseDocumentJsonStream(StreamReader sr)
    {
        List<Document> documents = new List<Document>();

        try
        {
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        continue;
                    }

                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject jsonObj = JObject.Load(reader);

                        var jsonNode = jsonObj.ToObject<DocumentJsonNode>();

                        if (jsonNode != null)
                        {
                            List<string> CorpusList = jsonNode.Corpus
                                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .AsEnumerable<string>()
                                .ToList();
                            List<string> KeywordList = jsonNode.Keywords
                                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .AsEnumerable<string>()
                                .ToList();

                            var document = new Document
                            {
                                Name = jsonNode.DocumentName,
                                Location = jsonNode.DocumentLocation,
                                Language = jsonNode.Language,
                                TopicProbabilty = jsonNode.TopicProbability
                            };
                            var topic = new DocumentTopic
                            {
                                Number = jsonNode.TopicNumber,
                                Document = document
                            };
                            var keywords = new List<DocumentKeyword>();
                            foreach (var keyword in KeywordList)
                            {
                                bool isInCorpus = false;
                                List<string> keywordParts = keyword.Split(' ').ToList();
                                // if it's a bigram, need to check next word in corpus
                                if (keywordParts.Count == 2)
                                {
                                    var matchIndex = CorpusList.FindIndex(x => x == keywordParts[0]);
                                    while (matchIndex != -1)
                                    {
                                        if (CorpusList[matchIndex + 1] == keywordParts[1])
                                        {
                                            isInCorpus = true;
                                            break;
                                        }

                                        matchIndex = CorpusList.FindIndex(matchIndex + 1, x => x == keywordParts[0]);
                                    }
                                }
                                else
                                {
                                    isInCorpus = CorpusList.Contains(keyword);
                                }

                                keywords.Add(new DocumentKeyword
                                {
                                    Name = keyword,
                                    IsInCorpus = isInCorpus,
                                    DocumentTopic = topic,
                                    Document = document
                                });
                            };

                            document.DocumentTopic = topic;
                            document.DocumentKeywords = keywords;

                            documents.Add(document);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(ModelJsonParser::ParseDocumentJsonStream) -- JSON Parse Error -- {ex}");
        }

        return documents;
    }

    public static Document? ParseDocumentJsonString(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        try
        {
            var jsonNode = JsonConvert.DeserializeObject<DocumentJsonNode>(json);
            if (jsonNode != null)
            {
                List<string> CorpusList = jsonNode.Corpus
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .AsEnumerable<string>()
                    .ToList();
                List<string> KeywordList = jsonNode.Keywords
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .AsEnumerable<string>()
                    .Distinct()
                    .ToList();
                var document = new Document
                {
                    Name = Path.GetFileName(jsonNode.DocumentName),
                    Location = jsonNode.DocumentLocation,
                    Language = jsonNode.Language,
                    TopicProbabilty = jsonNode.TopicProbability
                };
                var topic = new DocumentTopic
                {
                    Number = jsonNode.TopicNumber,
                    Document = document
                };
                var keywords = new List<DocumentKeyword>();
                foreach (var keyword in KeywordList)
                {
                    bool isInCorpus = false;
                    List<string> keywordParts = keyword.Split(' ').ToList();
                    // if it's a bigram, need to check next word in corpus
                    if (keywordParts.Count == 2)
                    {
                        var matchIndex = CorpusList.FindIndex(x => x == keywordParts[0]);
                        while (matchIndex != -1 && matchIndex < CorpusList.Count - 1)
                        {
                            if (CorpusList[matchIndex + 1] == keywordParts[1])
                            {
                                isInCorpus = true;
                                break;
                            }

                            matchIndex = CorpusList.FindIndex(matchIndex + 1, x => x == keywordParts[0]);
                        }
                    }
                    else
                    {
                        isInCorpus = CorpusList.Contains(keyword);
                    }

                    keywords.Add(new DocumentKeyword
                    {
                        Name = keyword,
                        IsInCorpus = isInCorpus,
                        DocumentTopic = topic,
                        Document = document
                    });
                };

                document.DocumentTopic = topic;
                document.DocumentKeywords = keywords;

                return document;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(ModelJsonParser::ParseDocumentJsonString) -- JSON Parse Error -- {ex}");
        }

        return null;
    }

    public static List<Video> ParseVideoJsonStream(StreamReader sr)
    {
        List<Video> videos = new List<Video>();

        try
        {
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        continue;
                    }

                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject jsonObj = JObject.Load(reader);

                        var jsonNode = jsonObj.ToObject<VideoJsonNode>();

                        if (jsonNode != null)
                        {
                            List<string> KeywordList = jsonNode.Keywords
                                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .AsEnumerable<string>()
                                .ToList();
                            List<string> ClipTextList = jsonNode.ClipText.ToList();
                            List<float> ClipStartList = jsonNode.ClipStart.ToList();
                            List<float> ClipDurationList = jsonNode.ClipDuration.ToList();

                            var video = new Video
                            {
                                Name = Path.GetFileName(jsonNode.VideoName),
                                Location = jsonNode.VideoLocation,
                                Language = jsonNode.Language,
                                TopicProbabilty = jsonNode.TopicProbability
                            };
                            var topic = new VideoTopic
                            {
                                Number = jsonNode.TopicNumber,
                                Video = video
                            };
                            var keywords = new List<VideoKeyword>();
                            foreach (var keyword in KeywordList)
                            {
                                var clip = ClipTextList
                                    .Select((s, i) => new { clipText = s, index = i })
                                    .Where(x => x.clipText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                    .Select(s => new
                                    {
                                        s.clipText,
                                        word = keyword,
                                        s.index,
                                        clipStart = ClipStartList.ElementAt(s.index),
                                        clipDuration = ClipDurationList.ElementAt(s.index)
                                    })
                                    .ToList();

                                if (clip.Count > 0)
                                {
                                    foreach (var c in clip)
                                    {
                                        keywords.Add(new VideoKeyword
                                        {
                                            Name = keyword,
                                            IsInClipText = true,
                                            ClipStart = (decimal)c.clipStart,
                                            ClipDuration = (decimal)c.clipDuration,
                                            VideoTopic = topic,
                                            Video = video
                                        });
                                    }
                                }
                                else
                                {
                                    keywords.Add(new VideoKeyword
                                    {
                                        Name = keyword,
                                        IsInClipText = false,
                                        ClipStart = 0m,
                                        ClipDuration = 0m,
                                        VideoTopic = topic,
                                        Video = video
                                    });
                                }
                            };

                            video.VideoTopic = topic;
                            video.VideoKeywords = keywords;

                            videos.Add(video);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(ModelJsonParser::ParseVideoJsonStream) -- JSON Parse Error -- {ex}");
        }

        return videos;
    }

    public static Video? ParseVideoJsonString(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        try
        {
            var jsonNode = JsonConvert.DeserializeObject<VideoJsonNode>(json);
            if (jsonNode != null)
            {
                List<string> KeywordList = jsonNode.Keywords
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .AsEnumerable<string>()
                    .Distinct()
                    .ToList();
                List<string> ClipTextList = jsonNode.ClipText.ToList();
                List<float> ClipStartList = jsonNode.ClipStart.ToList();
                List<float> ClipDurationList = jsonNode.ClipDuration.ToList();

                var video = new Video
                {
                    Name = Path.GetFileName(jsonNode.VideoName),
                    Location = jsonNode.VideoLocation,
                    Language = jsonNode.Language,
                    TopicProbabilty = jsonNode.TopicProbability
                };
                var topic = new VideoTopic
                {
                    Number = jsonNode.TopicNumber,
                    Video = video
                };
                var keywords = new List<VideoKeyword>();
                List<string> checkWords = new List<string> { "status", "arbeit", "markieren", "anzeigen lassen" };
                foreach (var keyword in KeywordList)
                {
                    var regex = new Regex($"\\b{keyword}\\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    var clip = ClipTextList
                        .Select((s, i) => new { clipText = s, index = i })
                        .Where(x => regex.IsMatch(x.clipText))
                        .Select(s => new
                        {
                            s.clipText,
                            word = keyword,
                            s.index,
                            clipStart = ClipStartList.ElementAt(s.index),
                            clipDuration = ClipDurationList.ElementAt(s.index)
                        })
                        .ToList();

                    if (clip.Count > 0)
                    {
                        foreach (var c in clip)
                        {
                            keywords.Add(new VideoKeyword
                            {
                                Name = keyword,
                                IsInClipText = true,
                                ClipStart = (decimal)c.clipStart,
                                ClipDuration = (decimal)c.clipDuration,
                                VideoTopic = topic,
                                Video = video
                            });
                        }
                    }
                    else
                    {
                        keywords.Add(new VideoKeyword
                        {
                            Name = keyword,
                            IsInClipText = false,
                            ClipStart = 0m,
                            ClipDuration = 0m,
                            VideoTopic = topic,
                            Video = video
                        });
                    }
                };

                video.VideoTopic = topic;
                video.VideoKeywords = keywords;

                return video;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(ModelJsonParser::ParseVideoJsonString) -- JSON Parse Error -- {ex}");
        }

        return null;
    }

    private class DocumentJsonNode
    {
        public string DocumentName { get; set; }
        public string DocumentLocation { get; set; }
        public string Language { get; set; }
        public int TopicNumber { get; set; }
        public decimal TopicProbability { get; set; }
        public string Keywords { get; set; }
        public string Corpus { get; set; }
    }

    private class VideoJsonNode
    {
        public string VideoName { get; set; }
        public string VideoLocation { get; set; }
        public string Language { get; set; }
        public int TopicNumber { get; set; }
        public decimal TopicProbability { get; set; }
        public string Keywords { get; set; }
        public List<string> ClipText { get; set; }
        public List<float> ClipStart { get; set; }
        public List<float> ClipDuration { get; set; }
    }
}
