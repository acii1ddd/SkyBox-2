using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Auth;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Abstractions.Jwt;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AccountController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public AccountController(IAuthService authService, IMapper mapper, IJwtService jwtService)
    {
        _authService = authService;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="signInRequest"></param>
    [AllowAnonymous]
    [HttpPost("sign-in")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest signInRequest)
    {
        var signInModel = _mapper.Map<SignInModel>(signInRequest);
        var authTokenModelResult = await _authService.SignIn(signInModel);
        
        if (authTokenModelResult.IsFailed)
        {
            // с ошибкой - объект BadRequestObjectResult, а просто вот так 
            // BadRequest() - BadRequestResult
            return BadRequest(authTokenModelResult.Errors);
        }

        SetRefreshTokenToCookie(authTokenModelResult.Value.RefreshToken);
        
        var result = _mapper.Map<SignInResponse>(authTokenModelResult.Value.AuthAccessTokenModel);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("refresh-tokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    public async Task<IActionResult> RefreshTokens()
    {
        var refreshToken = Request.Cookies["RefreshToken"];
        if (refreshToken is null)
        {
            return Unauthorized(); // need to relogin in app
        }
        
        var authTokenModelResult = await _jwtService.GetNewTokensPair(refreshToken);
        if (authTokenModelResult.IsFailed)
        {
            return BadRequest(authTokenModelResult.Errors);
        }
        
        SetRefreshTokenToCookie(authTokenModelResult.Value.RefreshToken);
        
        var result = _mapper.Map<SignInResponse>(authTokenModelResult.Value.AuthAccessTokenModel);
        return Ok(result);
    }
    
    // refresh токен кончится тогда, когда в RefreshTokens не будут
    // приходит запросы в течение времени действия refresh токена (Expires)
    private void SetRefreshTokenToCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            // СДЕЛАТЬ ВРЕМЯ БОЛЬШЕ И ВЫНЕСТИ В КОНФИГУРАЦИЮ
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        
        Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }
    
    // [AllowAnonymous]
    // [Authorize("Default")]
    // [HttpPost]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenModel))]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task Register([FromBody] SignInModel signInModel)
    // {
    //     throw new NotImplementedException("This functionality is not implemented");
    // }
}
