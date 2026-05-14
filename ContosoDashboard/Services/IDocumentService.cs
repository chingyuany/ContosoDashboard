using System.Security.Claims;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
	Task<Document?> UploadDocumentAsync(DocumentUploadModel request, ClaimsPrincipal user);
	Task<List<Document>> GetMyDocumentsAsync(ClaimsPrincipal user, string? searchTerm = null, int? projectId = null, bool sortDescending = true);
	Task<List<Document>> GetProjectDocumentsAsync(int projectId, ClaimsPrincipal user);
	Task<Document?> GetDocumentByIdAsync(int documentId, ClaimsPrincipal user);
}
