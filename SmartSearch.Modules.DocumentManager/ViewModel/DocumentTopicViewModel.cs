using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Modules.DocumentManager.ViewModel;
public class DocumentTopicViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    [Required]
    public int DocumentId { get; set; }

    public DocumentViewModel Document { get; set; }
    public ICollection<DocumentKeywordViewModel> DocumentKeywords { get; set; }
}
