using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Models.Auth;

public class GenerateTokenPayload
{
    public Guid UserId { get; set; }
    
    public UserRole UserRole { get; set; }
}