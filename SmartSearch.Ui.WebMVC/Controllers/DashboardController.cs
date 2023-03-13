using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartSearch.Modules.DocumentManager.Service;
using SmartSearch.Modules.VideoManager.Service;
using SmartSearch.Ui.WebMVC.Models;
using SmartSearch.UI.WebMVC.ViewModel;
using System.Diagnostics;


namespace SmartSearch.Ui.WebMVC.Controllers;

public class DashboardController : Controller
{
    private readonly IDocumentService _documentService;
    private readonly IDocumentKeywordService _documentKeywordService;
    private readonly IVideoService _videoService;
    private readonly IVideoKeywordService _videoKeywordService;

    public DashboardController(IDocumentService documentService, IDocumentKeywordService documentKeywordService, IVideoService videoService, IVideoKeywordService videoKeywordService)
    {
        _documentService = documentService;
        _documentKeywordService = documentKeywordService;
        _videoService = videoService;
        _videoKeywordService = videoKeywordService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public JsonResult GetDocumentsAndVideosForKeyword(string keyword)
    {
        Dictionary<string, List<SearchResultViewModel>> response = new Dictionary<string, List<SearchResultViewModel>>();

        try
        {
            List<SearchResultViewModel> documentCombinedList = new List<SearchResultViewModel>();

            var documentKeywords = _documentKeywordService.GetAllIncluding(keyword);
            documentCombinedList.AddRange(documentKeywords.Select(s => new SearchResultViewModel
            {
                Id = s.DocumentId,
                Name = s.Document.Name,
                Location = s.Document.Location,
                Language = s.Document.Language,
                TopicNumber = s.TopicId,
                TopicProbabilty = s.Document.TopicProbabilty,
                IsInCorpus = s.IsInCorpus,
                IsInClipText = null,
                ClipStart = null,
                ClipDuration = null
            }));
            response.Add("Document", documentCombinedList.OrderByDescending(s => s.TopicProbabilty).ToList());

            List<SearchResultViewModel> videoCombinedList = new List<SearchResultViewModel>();
            var videos = _videoService.GetByKeyword(keyword);
            videoCombinedList.AddRange(videos
                .Where(x => x.VideoKeywords.Count > 0)
                .Select(s => new SearchResultViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    Language = s.Language,
                    TopicNumber = s.VideoTopic.Id,
                    TopicProbabilty = s.TopicProbabilty,
                    IsInCorpus = null,
                    IsInClipText = s.VideoKeywords.Select(s => s.IsInClipText).FirstOrDefault(),
                    ClipStart = s.VideoKeywords.Select(s => s.ClipStart).ToList(),
                    ClipDuration = s.VideoKeywords.Select(s => s.ClipDuration).ToList()
                }));
            response.Add("Video", videoCombinedList.OrderByDescending(s => s.TopicProbabilty).ToList());

            return Json(new { success = true, data = response });
        }
        catch (Exception ex)
        {
            Log.Error($"(DashboardController::GetDocumentsAndVideosForKeyword) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpPost]
    public JsonResult GetKeywordListForAutocomplete(string searchText)
    {
        List<string> dataset = new List<string>();

        try
        {
            var documentKeywords = _documentKeywordService.GetAllDistinct(searchText);
            dataset.AddRange(documentKeywords);

            var videoKeywords = _videoKeywordService.GetAllDistinct(searchText);
            dataset.AddRange(videoKeywords);

            return Json(new { success = true, data = dataset.Distinct().OrderBy(q => q).ToList() });
        }
        catch (Exception ex)
        {
            Log.Error($"(DashboardController::GetKeywordListForAutocomplete) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }
}