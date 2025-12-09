namespace yeni.Configuration;

public class B2StorageOptions
{
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string ServiceUrl { get; set; } = null!;
    public string PublicBaseUrl { get; set; } = null!;
}