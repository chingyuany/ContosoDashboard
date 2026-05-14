using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
	[Key]
	public int DocumentId { get; set; }

	[Required]
	[MaxLength(255)]
	public string Title { get; set; } = string.Empty;

	[MaxLength(2000)]
	public string? Description { get; set; }

	[Required]
	[MaxLength(100)]
	public string Category { get; set; } = string.Empty;

	[Required]
	[MaxLength(255)]
	public string FileType { get; set; } = string.Empty;

	[Required]
	[MaxLength(500)]
	public string FilePath { get; set; } = string.Empty;

	[Required]
	[MaxLength(500)]
	public string OriginalFileName { get; set; } = string.Empty;

	[Required]
	public long FileSize { get; set; }

	[Required]
	public DateTime UploadDate { get; set; } = DateTime.UtcNow;

	[Required]
	public int UploadedByUserId { get; set; }

	public int? AssociatedProjectId { get; set; }

	[ForeignKey("UploadedByUserId")]
	public virtual User UploadedByUser { get; set; } = null!;

	[ForeignKey("AssociatedProjectId")]
	public virtual Project? AssociatedProject { get; set; }

	public virtual ICollection<DocumentTag> Tags { get; set; } = new List<DocumentTag>();
	public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
	public virtual ICollection<DocumentActivity> Activities { get; set; } = new List<DocumentActivity>();
	public virtual ICollection<TaskDocument> TaskAttachments { get; set; } = new List<TaskDocument>();
}
