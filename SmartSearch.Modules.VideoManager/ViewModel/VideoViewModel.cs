using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Modules.VideoManager.ViewModel;
public class VideoViewModel
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

    public VideoTopicViewModel VideoTopic { get; set; }
    public ICollection<VideoKeywordViewModel> VideoKeywords { get; set; }
}
