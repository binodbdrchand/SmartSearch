using System.ComponentModel.DataAnnotations;


namespace SmartSearch.Modules.DocumentManager.ViewModel;

public class DocumentViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public string Language { get; set; }

    [Required]
    public decimal TopicProbabilty { get; set; }

    public DocumentTopicViewModel DocumentTopic { get; set; }
    public ICollection<DocumentKeywordViewModel> DocumentKeywords { get; set; }
}
