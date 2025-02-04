using SkyBox.Domain.Models.User;

namespace SkyBox.API.Contracts.Auth;

public class SignInResponse
{
    public Guid UserId { get; init; }
    
    public UserRole UserRole { get; init; }
    
    public string AccessToken { get; init; } = string.Empty;
    
    public DateTime Expires { get; init; }
}
