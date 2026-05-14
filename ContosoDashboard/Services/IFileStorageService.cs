namespace ContosoDashboard.Services;

public interface IFileStorageService
{
	Task<string> SaveFileAsync(Stream content, string fileName, string contentType);
	Task<Stream> OpenReadAsync(string storedFileName);
	Task<bool> DeleteFileAsync(string storedFileName);
	Task<bool> FileExistsAsync(string storedFileName);
}
