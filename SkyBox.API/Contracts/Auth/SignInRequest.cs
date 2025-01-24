namespace SkyBox.API.Contracts.Auth;

public record SignInRequest
{
    public string UserName { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}
