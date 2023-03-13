using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.VideoManager.Domain;
public class VideoKeyword : EntityBase<int>
{
    public string Name { get; set; }
    public bool IsInClipText { get; set; }
    public decimal ClipStart { get; set; }
    public decimal ClipDuration { get; set; }
    public virtual int TopicId { get; set; }
    public virtual int VideoId { get; set; }

    public virtual Video Video { get; set; }
    public virtual VideoTopic VideoTopic { get; set; }
}