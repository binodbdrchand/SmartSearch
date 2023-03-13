using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Modules.VideoManager.ViewModel;
public class VideoKeywordViewModel
{
    [Required]
    public string Name { get; set; }

    [Required] 
    public bool IsInClipText { get; set; }

    [Required]
    public decimal ClipStart { get; set; }

    [Required]
    public decimal ClipDuration { get; set; }

    [Required]
    public int TopicId { get; set; }

    [Required]
    public int VideoId { get; set; }

    public VideoTopicViewModel VideoTopic { get; set; }
    public VideoViewModel Video { get; set; }
}
