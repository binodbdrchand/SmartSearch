using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Modules.DocumentManager.ViewModel;
public class DocumentKeywordViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public bool IsInCorpus { get; set; }

    [Required]
    public int TopicId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    public DocumentTopicViewModel DocumentTopic { get; set; }
    public DocumentViewModel Document { get; set; }
}
