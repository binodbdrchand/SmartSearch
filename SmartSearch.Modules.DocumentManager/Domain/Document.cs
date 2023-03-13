using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.DocumentManager.Domain;
public class Document : EntityBase<int>
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Language { get; set; }
    public decimal TopicProbabilty { get; set; }

    public virtual DocumentTopic DocumentTopic { get; set; }
    public virtual ICollection<DocumentKeyword> DocumentKeywords { get; set; }
}
