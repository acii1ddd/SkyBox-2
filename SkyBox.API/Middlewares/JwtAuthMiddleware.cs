using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace SkyBox.API.Middlewares;

public class JwtAuthMiddleware
{
    public readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthMiddleware> _logger;

    public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // для перехвата jwt токена 
    public async Task Invoke(HttpContext context)
    {
        var bearerToken = context.Request
            .Headers
            .Authorization
            .ToString();

        if (string.IsNullOrEmpty(bearerToken))
        {
            await _next(context);
        }
        
        var token = bearerToken.Split(" ").Last();

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters();
        
        var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);
        
        if (!validationResult.IsValid)
        {
            _logger.LogError("{Source}: Invalid jwt token: ", 
                nameof(JwtAuthMiddleware));
        }

        if (validationResult.Exception != null)
        {
            _logger.LogError("{Source}: An error occurred while validating " +
                             "the token. Message: {ExceptionMessage}", 
                nameof(JwtAuthMiddleware),
                validationResult.Exception.Message);
        }
        
        await _next(context);
    }
}