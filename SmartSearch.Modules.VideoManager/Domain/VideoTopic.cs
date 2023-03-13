using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.VideoManager.Domain;
public class VideoTopic : EntityBase<int>
{
    public int Number { get; set; }
    public virtual int VideoId { get; set; }

    public virtual Video Video { get; set; }
    public virtual ICollection<VideoKeyword> VideoKeywords { get; set; }
}
