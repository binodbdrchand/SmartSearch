using Serilog;
using SmartSearch.Worker.FileMonitorService.Helpers;
using SmartSearch.Worker.FileMonitorService.Models.Document;
using SmartSearch.Worker.FileMonitorService.Models.Video;
using SmartSearch.Worker.FileMonitorService.Services.Repository;


namespace SmartSearch.Worker.FileMonitorService.Services;

public class FileWatcher
{
    private readonly FileSystemWatcher _watcher;
    private readonly DocumentRepository _documentRepository;
    private readonly VideoRepository _videoRepository;

    public FileWatcher(string folderToWatch)
    {
        _watcher = new FileSystemWatcher(folderToWatch);
        _watcher.IncludeSubdirectories = true;

        // extension
        _watcher.Filter = "*.*";

        // events & event handlers
        _watcher.Created += OnCreated;
        _watcher.Renamed += OnRenamed;
        _watcher.Deleted += OnDeleted;

        // fire events
        _watcher.EnableRaisingEvents = true;
        // for large number of files created at once
        _watcher.InternalBufferSize = 65536;

        _documentRepository = new DocumentRepository(AppStaticValues.SqlServerDbContextOptions());
        _videoRepository = new VideoRepository(AppStaticValues.SqlServerDbContextOptions());
    }

    public void OnCreated(object sender, FileSystemEventArgs e)
    {
        Log.Information($"(FileWatcher::FileCreated) -- Filename: {e.Name}");

        if (e != null && e.Name != null)
        {
            string fileExt = Path.GetExtension(e.Name).ToLower().Replace(".", string.Empty);

            if (fileExt != null)
            {
                if (AppStaticValues.DocumentExtensions().Contains(fileExt))
                {
                    var response = AIModel.ProcessDocument(e.FullPath);
                    if (response != null && response != string.Empty)
                    {
                        var document = ModelJsonParser.ParseDocumentJsonString(response);

                        if (document != null)
                        {
                            _documentRepository.Insert(new List<Document> { document });
                        }
                    }
                }
                else if (AppStaticValues.VideoExtensions().Contains(fileExt))
                {
                    var response = AIModel.ProcessVideo(e.FullPath);
                    if (response != null && response != string.Empty)
                    {
                        var video = ModelJsonParser.ParseVideoJsonString(response);
                        if (video != null)
                        {
                            _videoRepository.Insert(new List<Video> { video });
                        }
                    }
                }
            }
        }
    }

    public void OnRenamed(object sender, RenamedEventArgs e)
    {
        Log.Information($"(FileWatcher::FileRenamed) -- -- From: {e.OldName}, To: {e.Name}");

        if (e.OldName != null && e.Name != null)
        {
            var fileToBeDeleted = e.OldName.Split(new char[] { '\\' }).ToList().LastOrDefault();            
            string fileExt = Path.GetExtension(e.Name).ToLower().Replace(".", string.Empty);

            if (fileToBeDeleted != null && fileExt != null)
            {
                if (AppStaticValues.DocumentExtensions().Contains(fileExt))
                {
                    var response = AIModel.ProcessDocument(e.FullPath);
                    if (response != null && response != string.Empty)
                    {
                        var document = ModelJsonParser.ParseDocumentJsonString(response);

                        if (document != null)
                        {
                            _documentRepository.Delete(fileToBeDeleted);
                            _documentRepository.Insert(new List<Document> { document });
                        }
                    }
                }
                else if (AppStaticValues.VideoExtensions().Contains(fileExt))
                {
                    var response = AIModel.ProcessVideo(e.FullPath);
                    if (response != null && response != string.Empty)
                    {
                        var video = ModelJsonParser.ParseVideoJsonString(response);
                        if (video != null)
                        {
                            _videoRepository.Delete(fileToBeDeleted);
                            _videoRepository.Insert(new List<Video> { video });
                        }
                    }
                }
            }
        }
    }

    public void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (e.Name != null)
        {
            Log.Information($"(FileWatcher::FileDeleted) -- Filename: {e.Name}");

            var fileToBeDeleted = e.Name.Split(new char[] { '\\' }).ToList().LastOrDefault();
            string fileExt = Path.GetExtension(e.Name).ToLower().Replace(".", string.Empty);

            if (fileToBeDeleted != null && fileExt != null)
            {
                if (AppStaticValues.DocumentExtensions().Contains(fileExt))
                {
                    _documentRepository.Delete(fileToBeDeleted);
                }
                else if (AppStaticValues.VideoExtensions().Contains(fileExt))
                {
                    _videoRepository.Delete(fileToBeDeleted);
                }
            }
        }
    }
}
