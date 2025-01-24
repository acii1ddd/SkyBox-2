namespace SkyBox.FileStorageConfiguration.Auth;

public class AuthSettings
{
    public InternalAuthSettings Internal { get; set; }
}

public class InternalAuthSettings
{
    public string Issuer { get; set; } = string.Empty;
    
    public string Audience { get; set; } = string.Empty;
    
    public int LifeTime { get; set; }
    
    public string Secret { get; set; } = string.Empty;
}
