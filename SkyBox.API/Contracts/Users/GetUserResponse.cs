namespace SkyBox.API.Contracts.Users;

public record GetUserResponse
{
    public Guid Id { get; init; }
    
    public string UserName { get; init; } = string.Empty;
    
    public string Email { get; init; } = string.Empty;
}