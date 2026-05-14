using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public class DocumentService : IDocumentService
{
	private static readonly Dictionary<string, string> AllowedMimeTypes = new()
	{
		{ ".pdf", "application/pdf" },
		{ ".doc", "application/msword" },
		{ ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
		{ ".xls", "application/vnd.ms-excel" },
		{ ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
		{ ".ppt", "application/vnd.ms-powerpoint" },
		{ ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
		{ ".txt", "text/plain" },
		{ ".jpg", "image/jpeg" },
		{ ".jpeg", "image/jpeg" },
		{ ".png", "image/png" }
	};

	private static readonly List<string> AllowedCategories = new()
	{
		"Project Documents",
		"Team Resources",
		"Personal Files",
		"Reports",
		"Presentations",
		"Other"
	};

	private const long MaxFileSizeBytes = 25L * 1024L * 1024L;

	private readonly ApplicationDbContext _context;
	private readonly IFileStorageService _fileStorageService;
	private readonly IUserService _userService;
	private readonly INotificationService _notificationService;

	public DocumentService(
		ApplicationDbContext context,
		IFileStorageService fileStorageService,
		IUserService userService,
		INotificationService notificationService)
	{
		_context = context;
		_fileStorageService = fileStorageService;
		_userService = userService;
		_notificationService = notificationService;
	}

	public async Task<Document?> UploadDocumentAsync(DocumentUploadModel request, ClaimsPrincipal user)
	{
		if (request.File == null)
		{
			throw new InvalidOperationException("A document file is required for upload.");
		}

		var userId = GetCurrentUserId(user);
		if (userId == null)
			throw new UnauthorizedAccessException("User must be authenticated to upload documents.");

		var currentUser = await _userService.GetUserByIdAsync(userId.Value);
		if (currentUser == null)
			throw new UnauthorizedAccessException("Authenticated user not found.");

		if (string.IsNullOrWhiteSpace(request.Title))
			throw new InvalidOperationException("Document title is required.");

		if (!AllowedCategories.Contains(request.Category))
			throw new InvalidOperationException($"Invalid category: {request.Category}");

		if (request.File.Size > MaxFileSizeBytes)
			throw new InvalidOperationException("File exceeds maximum size of 25 MB");

		var extension = Path.GetExtension(request.File.Name).ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(extension) || !AllowedMimeTypes.ContainsKey(extension))
			throw new InvalidOperationException("File type not supported. Allowed: PDF, Office documents, text, images");

		var allowedMimeType = AllowedMimeTypes[extension];
		if (!string.Equals(request.File.ContentType, allowedMimeType, StringComparison.OrdinalIgnoreCase) && request.File.ContentType != "application/octet-stream")
			throw new InvalidOperationException("File type not supported. Allowed: PDF, Office documents, text, images");

		if (request.AssociatedProjectId.HasValue)
		{
			var canUpload = await CanUploadToProjectAsync(request.AssociatedProjectId.Value, currentUser);
			if (!canUpload)
				throw new UnauthorizedAccessException("You don't have permission to upload documents to this project");
		}

		await using var uploadStream = request.File.OpenReadStream(MaxFileSizeBytes);
		var storedFileName = await _fileStorageService.SaveFileAsync(uploadStream, request.File.Name, request.File.ContentType ?? allowedMimeType);

		var document = new Document
		{
			Title = request.Title,
			Description = request.Description,
			Category = request.Category,
			FileType = request.File.ContentType ?? allowedMimeType,
			FilePath = storedFileName,
			OriginalFileName = request.File.Name,
			FileSize = request.File.Size,
			UploadDate = DateTime.UtcNow,
			UploadedByUserId = currentUser.UserId,
			AssociatedProjectId = request.AssociatedProjectId
		};

		if (!string.IsNullOrWhiteSpace(request.Tags))
		{
			document.Tags = request.Tags
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(tag => new DocumentTag { Value = tag.Trim() })
				.ToList();
		}

		document.Activities.Add(new DocumentActivity
		{
			ActivityType = "Upload",
			UserId = currentUser.UserId,
			ActivityDate = DateTime.UtcNow,
			Details = $"Uploaded document '{document.Title}' ({document.OriginalFileName})"
		});

		_context.Documents.Add(document);
		await _context.SaveChangesAsync();

		return document;
	}

	public async Task<List<Document>> GetMyDocumentsAsync(ClaimsPrincipal user)
	{
		var userId = GetCurrentUserId(user);
		if (userId == null)
			return new List<Document>();

		return await _context.Documents
			.Include(d => d.AssociatedProject)
			.Where(d => d.UploadedByUserId == userId.Value)
			.OrderByDescending(d => d.UploadDate)
			.ToListAsync();
	}

	public async Task<List<Document>> GetProjectDocumentsAsync(int projectId, ClaimsPrincipal user)
	{
		var userId = GetCurrentUserId(user);
		if (userId == null)
			return new List<Document>();

		if (!await CanViewProjectDocumentsAsync(projectId, userId.Value))
			return new List<Document>();

		return await _context.Documents
			.Include(d => d.UploadedByUser)
			.Where(d => d.AssociatedProjectId == projectId)
			.OrderByDescending(d => d.UploadDate)
			.ToListAsync();
	}

	public async Task<Document?> GetDocumentByIdAsync(int documentId, ClaimsPrincipal user)
	{
		var userId = GetCurrentUserId(user);
		if (userId == null)
			return null;

		var document = await _context.Documents
			.Include(d => d.AssociatedProject)
			.Include(d => d.UploadedByUser)
			.FirstOrDefaultAsync(d => d.DocumentId == documentId);

		if (document == null)
			return null;

		if (!await IsAuthorizedForDocumentAsync(document, userId.Value))
			return null;

		return document;
	}

	private int? GetCurrentUserId(ClaimsPrincipal user)
	{
		var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
			?? user.FindFirst("sub")?.Value
			?? user.FindFirst(ClaimTypes.Name)?.Value;

		return int.TryParse(idClaim, out var result) ? result : null;
	}

	private async Task<bool> CanUploadToProjectAsync(int projectId, User currentUser)
	{
		if (currentUser.Role == UserRole.Administrator || currentUser.Role == UserRole.ProjectManager)
			return true;

		return await _context.ProjectMembers
			.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == currentUser.UserId);
	}

	private async Task<bool> CanViewProjectDocumentsAsync(int projectId, int userId)
	{
		var user = await _userService.GetUserByIdAsync(userId);
		if (user == null) return false;

		if (user.Role == UserRole.Administrator || user.Role == UserRole.ProjectManager)
			return true;

		return await _context.ProjectMembers
			.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
	}

	private async Task<bool> IsAuthorizedForDocumentAsync(Document document, int userId)
	{
		if (document.UploadedByUserId == userId)
			return true;

		if (document.AssociatedProjectId.HasValue)
		{
			return await CanViewProjectDocumentsAsync(document.AssociatedProjectId.Value, userId);
		}

		return false;
	}
}