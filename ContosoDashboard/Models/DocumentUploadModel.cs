using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace ContosoDashboard.Models;

public class DocumentUploadModel
{
	[Required]
	[MaxLength(255)]
	public string Title { get; set; } = string.Empty;

	[MaxLength(2000)]
	public string? Description { get; set; }

	[Required]
	[MaxLength(100)]
	public string Category { get; set; } = string.Empty;

	public int? AssociatedProjectId { get; set; }

	[MaxLength(500)]
	public string? Tags { get; set; }

	[Required]
	public IBrowserFile? File { get; set; }
}
