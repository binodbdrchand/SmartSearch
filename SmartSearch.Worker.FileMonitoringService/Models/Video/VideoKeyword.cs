using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSearch.Worker.FileMonitorService.Models.Video;

[Table("VideoKeyword")]
public class VideoKeyword
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public bool IsInClipText { get; set; }

    [Required]
    public decimal ClipStart { get; set; }

    [Required]
    public decimal ClipDuration { get; set; }

    [Required]
    [ForeignKey(nameof(VideoTopic))]
    public virtual int TopicId { get; set; }

    [Required]
    [ForeignKey(nameof(Video))]
    public virtual int VideoId { get; set; }

    [MaxLength(50)]
    public string CreatedBy { get; set; } = "Service";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    [MaxLength(50)]
    public string LastModifiedBy { get; set; } = "Service";
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public virtual Video Video { get; set; }
    public virtual VideoTopic VideoTopic { get; set; }
}
