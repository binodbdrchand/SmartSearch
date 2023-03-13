using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.VideoManager.Domain;
public class Video : EntityBase<int>
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Language { get; set; }
    public decimal TopicProbabilty { get; set; }

    public virtual VideoTopic VideoTopic { get; set; }
    public virtual ICollection<VideoKeyword> VideoKeywords { get; set; }
}
