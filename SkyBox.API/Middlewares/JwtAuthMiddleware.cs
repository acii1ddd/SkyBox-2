// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.IdentityModel.Tokens;
//
// namespace SkyBox.API.Middlewares;
//
// public class JwtAuthMiddleware
// {
//     public readonly RequestDelegate _next;
//     private readonly ILogger<JwtAuthMiddleware> _logger;
//
//     public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger)
//     {
//         _next = next;
//         _logger = logger;
//     }
//
//     // для перехвата jwt токена (как-то влияет на return из контроллера)
//     public async Task Invoke(HttpContext context)
//     {
//         // excepts
//         // if (context.Request.Path.StartsWithSegments("/api/auth/sign-in"))
//         // {
//         //     await _next(context);
//         //     return;
//         // }
//         
//         var bearerToken = context.Request
//             .Headers
//             .Authorization
//             .ToString();
//         
//         if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
//         {
//             _logger.LogError("Missing header or header does not starts with /'Bearer/'.");
//             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             
//             await context.Response.WriteAsync("Unauthorized.");
//             return;
//         }
//         
//         var token = bearerToken.Split(" ").Last();
//         
//         var tokenHandler = new JwtSecurityTokenHandler();
//         var validationParameters = new TokenValidationParameters
//         {
//             
//         };
//         
//         var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);
//         
//         if (!validationResult.IsValid)
//         {
//             _logger.LogError("{Source}: Invalid jwt token: ",
//                 nameof(JwtAuthMiddleware));
//         }
//         
//         if (validationResult.Exception != null)
//         {
//             _logger.LogError("{Source}: An error occurred while validating " +
//                              "the token. Message: {ExceptionMessage}", 
//                 nameof(JwtAuthMiddleware),
//                 validationResult.Exception.Message);
//         }
//         
//         await _next(context);
//     }
// }