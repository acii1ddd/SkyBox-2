using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Auth;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AccountController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AccountController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
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

        var authTokenModel = authTokenModelResult.Value;
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            // СДЕЛАТЬ ВРЕМЯ БОЛЬШЕ
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        
        Response.Cookies.Append("RefreshToken", authTokenModel.RefreshToken, cookieOptions);
        
        var result = _mapper.Map<SignInResponse>(authTokenModelResult.Value.AuthAccessTokenModel);
        return Ok(result);
    }

    // [AllowAnonymous]
    // [HttpGet("refresh-tokens")]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    // public async Task<IActionResult> RefreshTokens()
    // {
    //     var refreshToken = Request.Cookies["RefreshToken"];
    //     
    //     // посмотреть есть ли в redis запись с ключом refreshToken
    //     bool isExists;
    //     if (isExists)
    //     {
    //         // отдать значение (guid соответствующего пользователя)
    //     }
    //     else
    //     {
    //         //        
    //     }
    // }
    
    
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
