using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartSearch.Modules.VideoManager.Service;


namespace SmartSearch.UI.WebMVC.Controllers;

public class VideoController : Controller
{
    private readonly IWebHostEnvironment _env;
    private IVideoService _videoService;
    private IVideoKeywordService _videoKeywordService;

    public VideoController(IWebHostEnvironment env, IVideoService videoService, IVideoKeywordService videoKeywordService)
    {
        _env = env;
        _videoService = videoService;
        _videoKeywordService = videoKeywordService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult GetKeywordListForAutocomplete(string searchText)
    {
        try
        {
            var result = _videoKeywordService.GetAllDistinct(searchText);

            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            Log.Error($"(VideoController::GetKeywordListForAutocomplete) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpPost]
    public JsonResult GetVideosForKeyword(string keyword)
    {
        try
        {
            var result = _videoKeywordService.GetAllIncluding(keyword);
            var videos = result.Select(s => s.Video);

            return Json(new { success = true, data = videos });
        }
        catch (Exception ex)
        {
            Log.Error($"(VideoController::GetVideosForKeyword) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult ViewVideo(int id, string keyword)
    {
        var result = _videoService.GetByIdAndKeyword(id, keyword);

        if (result != null)
        {
            var directoryInfo = new DirectoryInfo($"{_env.WebRootPath}/temp");
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            System.IO.File.Copy(result.Location, $"{_env.WebRootPath}/temp/{result.Name}", true);

            //var filePath = $"{_env.WebRootPath}/temp/{result.Name}";
            //if (!System.IO.File.Exists(filePath))
            //{
            //    System.IO.File.Copy(result.Location, filePath, true);
            //}

            ViewBag.FileName = result.Name;
            ViewBag.Keyword = keyword;

            if (result.VideoKeywords.Count > 0)
            {
                string jsonString = "[";
                foreach (var clipStart in result.VideoKeywords.Select(s => s.ClipStart).ToList()) 
                {
                    jsonString += "{";
                    jsonString += $"time: {clipStart}";
                    jsonString += "},";
                }
                jsonString += "]";

                ViewBag.ClipStartList = jsonString;
            }

            return View();
        }

        return RedirectToAction("Index");
    }
}
