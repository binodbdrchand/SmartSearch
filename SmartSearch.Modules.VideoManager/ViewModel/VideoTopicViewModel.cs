using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Modules.VideoManager.ViewModel;
public class VideoTopicViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    [Required]
    public int VideoId { get; set; }

    public VideoViewModel Video { get; set; }
    public ICollection<VideoKeywordViewModel> VideoKeywords { get; set; }
}
