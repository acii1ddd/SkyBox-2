using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Models.Auth;

public class AuthTokenModel
{
    public Guid UserId { get; set; }
    
    public UserRole UserRole { get; set; }
    
    public string Token { get; set; } = string.Empty;
    
    public DateTime Expires { get; set; }
}
