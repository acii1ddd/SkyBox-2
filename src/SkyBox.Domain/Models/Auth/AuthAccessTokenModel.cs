using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Models.Auth;

public class AuthAccessTokenModel
{
    public Guid UserId { get; set; }
    
    public UserRole UserRole { get; set; }
    
    public string AccessToken { get; set; } = string.Empty;
    
    public DateTime Expires { get; set; }
}