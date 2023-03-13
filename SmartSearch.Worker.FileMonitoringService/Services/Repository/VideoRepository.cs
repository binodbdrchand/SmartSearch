using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartSearch.Worker.FileMonitorService.Models.Video;
using SmartSearch.Worker.FileMonitorService.Persistence;

namespace SmartSearch.Worker.FileMonitorService.Services.Repository;

public class VideoRepository
{
    private readonly DbContextOptions<ServiceDbContext> _options;

    public VideoRepository(DbContextOptions<ServiceDbContext> options)
    {
        _options = options;
    }

    public Video? Get(string name)
    {
        using var dbContext = new ServiceDbContext(_options);

        return dbContext.Videos
            .Where(x => x.Name == name)
            .Include(i => i.VideoTopic)
            .Include(i => i.VideoKeywords)
            .FirstOrDefault();
    }

    public async Task<Video?> GetAsync(string name)
    {
        using var dbContext = new ServiceDbContext(_options);

        return await dbContext.Videos
            .Where(x => x.Name == name)
            .Include(i => i.VideoTopic)
            .Include(i => i.VideoKeywords)
            .FirstOrDefaultAsync();
    }

    public void Insert(List<Video> videos)
    {
        if (videos.Count > 0)
        {
            using var dbContext = new ServiceDbContext(_options);

            foreach (var video in videos)
            {
                bool inserted = false, updated = false;

                try
                {
                    var existingEntity = dbContext.Videos
                        .Where(x => x.Name == video.Name)
                        .Include(i => i.VideoTopic)
                        .Include(i => i.VideoKeywords)
                        .FirstOrDefault();

                    if (existingEntity == null)
                    {
                        dbContext.Videos.Add(video);

                        inserted = true;
                        updated = false;
                    }
                    else
                    {
                        if (video.VideoTopic != null)
                        {
                            existingEntity.VideoTopic = video.VideoTopic;
                            existingEntity.VideoTopic.Video = existingEntity;
                        }

                        if (video.VideoKeywords != null && video.VideoKeywords.Count > 0)
                        {
                            existingEntity.VideoKeywords = video.VideoKeywords;

                            foreach (var keyword in existingEntity.VideoKeywords)
                            {
                                keyword.Video = existingEntity;
                            }
                        }

                        dbContext.Update(existingEntity);

                        inserted = false;
                        updated = true;
                    }

                    dbContext.SaveChanges();

                    if (inserted)
                    {
                        Log.Information($"(VideoRepository::Insert) -- Record Inserted -- Id: {video.Id}, Name: {video.Name}");
                    }
                    if (updated)
                    {
                        Log.Information($"(VideoRepository::Insert) -- Record Updated -- Id: {existingEntity?.Id}, Name: {existingEntity?.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"(VideoRepository::Insert) -- INSERT Error -- {ex}");

                    continue;
                }
            }
        }
    }

    public void InsertRange(List<Video> videos)
    {
        if (videos.Count > 0)
        {
            using var dbContext = new ServiceDbContext(_options);

            try
            {
                dbContext.AddRange(videos);
                dbContext.SaveChanges();

                videos.ForEach(v => Log.Information($"(VideoRepository::InsertRange) -- Record Inserted -- Id: {v.Id}, Name: {v.Name}"));
            }
            catch (Exception ex)
            {
                Log.Error($"(VideoRepository::InsertRange) -- INSERT Error -- {ex}");
            }
        }
    }

    public async void InsertRangeAsync(List<Video> videos)
    {
        if (videos.Count > 0)
        {
            try
            {
                using var dbContext = new ServiceDbContext(_options);

                await dbContext.AddRangeAsync(videos);
                await dbContext.SaveChangesAsync();

                videos.ForEach(v => Log.Information($"(VideoRepository::InsertRangeAsync) -- Record Inserted -- Id: {v.Id}, Name: {v.Name}"));
            }
            catch (Exception ex)
            {
                Log.Error($"(VideoRepository::InsertRangeAsync) -- INSERT Error -- {ex}");
            }
        }
    }

    public void Delete(string name)
    {
        try
        {
            using var dbContext = new ServiceDbContext(_options);

            var entity = dbContext.Videos.Where(x => x.Name == name).FirstOrDefault();
            if (entity != null)
            {
                dbContext.Videos.Remove(entity);

                dbContext.SaveChanges();

                Log.Information($"(VideoRepository::Delete) -- Record Deleted -- Id: {entity.Id}, Name: {entity.Name}");
            }
            else 
            {
                Log.Warning($"(VideoRepository::Delete) -- DELETE Error -- Record Not Found -- Name: {name}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"(VideoRepository::Delete) -- DELETE Error -- {ex}");
        }
    }
}
