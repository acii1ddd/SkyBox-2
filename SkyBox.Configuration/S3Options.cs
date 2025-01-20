namespace SkyBox.FileStorageConfiguration;

/// <summary>
/// Настройки хранилища S3
/// </summary>
public class S3Options
{
    public string Region { get; set; } = string.Empty;
    
    public string ServiceUrl { get; set; } = string.Empty;
    
    public bool ForcePathStyle { get; set; }
    
    public bool UseHttp { get; set; }
    
    public string AccessKey { get; set; } = string.Empty;
    
    public string SecretKey { get; set; } = string.Empty;
}