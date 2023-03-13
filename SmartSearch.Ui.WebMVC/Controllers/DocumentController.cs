using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartSearch.Modules.DocumentManager.Service;


namespace SmartSearch.UI.WebMVC.Controllers;

public class DocumentController : Controller
{
    private IDocumentService _documentService;
    private IDocumentKeywordService _documentKeywordService;

    public DocumentController(IDocumentService documentService, IDocumentKeywordService documentKeywordService)
    {
        _documentService = documentService;
        _documentKeywordService = documentKeywordService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult GetDocumentsForKeyword(string keyword)
    {
        try
        {
            var result = _documentKeywordService.GetAllIncluding(keyword);
            var documents = result.Select(s => s.Document);

            return Json(new { success = true, data = documents });
        }
        catch (Exception ex)
        {
            Log.Error($"(DocumentController::GetDocumentsForKeyword) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpPost]
    public JsonResult GetKeywordListForAutocomplete(string searchText)
    {
        try
        {
            var result = _documentKeywordService.GetAllDistinct(searchText);

            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            Log.Error($"(DocumentController::GetKeywordListForAutocomplete) -- ERROR -- {ex}");
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult DownloadPdf(int id)
    {
        var result = _documentService.GetById(id);

        if (result != null)
        {
            byte[] pdfBytes = System.IO.File.ReadAllBytes(result.Location);
            MemoryStream pdfStream = new MemoryStream(pdfBytes);

            return File(pdfStream, "application/pdf", result.Name);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ViewPdf(int id)
    {
        var result = _documentService.GetById(id);

        if (result != null)
        {
            byte[] pdfBytes = System.IO.File.ReadAllBytes(result.Location);
            MemoryStream pdfStream = new MemoryStream(pdfBytes);

            Response.Headers.Add("Content-Disposition", "inline; filename=" + result.Name);

            return File(pdfStream, "application/pdf");
        }

        return RedirectToAction("Index");
    }
}
