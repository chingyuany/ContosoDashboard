using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
	private readonly string _storageRoot;
	private static readonly Regex InvalidPathChars = new("[^a-zA-Z0-9_\-.]", RegexOptions.Compiled);

	public LocalFileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
	{
		var configuredPath = configuration["FileStorage:UploadPath"] ?? "AppData/uploads";
		_storageRoot = Path.Combine(environment.ContentRootPath, configuredPath);
		Directory.CreateDirectory(_storageRoot);
	}

	public async Task<string> SaveFileAsync(Stream content, string fileName, string contentType)
	{
		var extension = Path.GetExtension(fileName);
		var sanitizedFileName = SanitizeFileName(Path.GetFileNameWithoutExtension(fileName));
		var storedFileName = $"{Guid.NewGuid():N}_{sanitizedFileName}{extension}";
		var destination = GetStoragePath(storedFileName);

		using var fileStream = File.Create(destination);
		await content.CopyToAsync(fileStream);

		return storedFileName;
	}

	public Task<Stream> OpenReadAsync(string storedFileName)
	{
		var path = GetStoragePath(storedFileName);
		return Task.FromResult<Stream>(File.OpenRead(path));
	}

	public Task<bool> DeleteFileAsync(string storedFileName)
	{
		var path = GetStoragePath(storedFileName);
		if (!File.Exists(path)) return Task.FromResult(false);

		File.Delete(path);
		return Task.FromResult(true);
	}

	public Task<bool> FileExistsAsync(string storedFileName)
	{
		var path = GetStoragePath(storedFileName);
		return Task.FromResult(File.Exists(path));
	}

	private string GetStoragePath(string storedFileName)
	{
		return Path.Combine(_storageRoot, storedFileName);
	}

	private static string SanitizeFileName(string fileName)
	{
		return InvalidPathChars.Replace(fileName, "_").Trim('_');
	}
}
