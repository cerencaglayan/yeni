using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace yeni.Configuration;

public class B2StorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName ;
    private readonly string _bucketUrl;

    public B2StorageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["B2Storage:BucketName"]!;
        _bucketUrl = configuration["B2Storage:ServiceUrl"]!;
    }

    public async Task<string> UploadFileAsync(IFormFile file, int userId, CancellationToken ct = default)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = $"users/{userId}/attachments/{uniqueFileName}";

        using var stream = file.OpenReadStream();

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = filePath,
            BucketName = _bucketName,
            ContentType = file.ContentType
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest, ct);

        return filePath; 
    }


    public async Task DeleteFileAsync(string filePath, CancellationToken ct = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = filePath
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, ct);
    }

    public string GetFileUrl(string filePath)
    {
        return $"{_bucketUrl}/file/{_bucketName}/{filePath}";
    }

    public async Task<Stream> DownloadFileAsync(string filePath, CancellationToken ct = default)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = filePath
        };

        var response = await _s3Client.GetObjectAsync(request, ct);
        return response.ResponseStream;
    }    
}