namespace yeni.Infrastructure.Configuration;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, int userId, CancellationToken ct = default);
    Task DeleteFileAsync(string filePath, CancellationToken ct = default);
    string GetFileUrl(string filePath);
    Task<Stream> DownloadFileAsync(string filePath, CancellationToken ct = default);
}