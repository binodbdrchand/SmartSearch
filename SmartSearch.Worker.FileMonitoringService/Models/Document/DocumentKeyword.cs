using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Worker.FileMonitorService.Models.Document;

[Table("DocumentKeyword")]
public class DocumentKeyword
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public bool IsInCorpus { get; set; }

    [Required]
    [ForeignKey(nameof(DocumentTopic))]
    public virtual int TopicId { get; set; }

    [Required]
    [ForeignKey(nameof(Document))]
    public virtual int DocumentId { get; set; }

    [MaxLength(50)]
    public string CreatedBy { get; set; } = "Service";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    [MaxLength(50)]
    public string LastModifiedBy { get; set; } = "Service";
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public virtual Document Document { get; set; }
    public virtual DocumentTopic DocumentTopic { get; set; }
}
