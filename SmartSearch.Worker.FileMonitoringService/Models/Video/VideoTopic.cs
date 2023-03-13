using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSearch.Worker.FileMonitorService.Models.Video;

[Table("VideoTopic")]
public class VideoTopic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

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
    public virtual ICollection<VideoKeyword> VideoKeywords { get; set; }
}
