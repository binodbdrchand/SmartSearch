using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.DocumentManager.Domain;
public class DocumentKeyword : EntityBase<int>
{
    public string Name { get; set; }
    public bool IsInCorpus { get; set; }
    public virtual int TopicId { get; set; }
    public virtual int DocumentId { get; set; }

    public virtual Document Document { get; set; }
    public virtual DocumentTopic DocumentTopic { get; set; }
}
