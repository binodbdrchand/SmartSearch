using SmartSearch.Core.Domain.Entity.Base;

namespace SmartSearch.Modules.DocumentManager.Domain;
public class DocumentTopic : EntityBase<int>
{
    public int Number { get; set; }
    public virtual int DocumentId { get; set; }

    public virtual Document Document { get; set; }
    public virtual ICollection<DocumentKeyword> DocumentKeywords { get; set; }
}
