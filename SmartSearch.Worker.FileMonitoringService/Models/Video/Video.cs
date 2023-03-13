using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSearch.Worker.FileMonitorService.Models.Video;

[Table("Video")]
public class Video
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Location { get; set; }

    [Required]
    [MaxLength(25)]
    public string Language { get; set; }

    [Required]
    public decimal TopicProbabilty { get; set; }

    [MaxLength(50)]
    public string CreatedBy { get; set; } = "Service";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    [MaxLength(50)]
    public string LastModifiedBy { get; set; } = "Service";
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public virtual VideoTopic VideoTopic { get; set; }
    public virtual ICollection<VideoKeyword> VideoKeywords { get; set; }
}
