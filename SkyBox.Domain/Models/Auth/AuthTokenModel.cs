namespace SkyBox.Domain.Models.Auth;

public class AuthTokenModel
{
    public AuthAccessTokenModel AuthAccessTokenModel { get; set; } = new();
    public string RefreshToken { get; set; } = string.Empty; // like new AuthAccessTokenModel();
}