using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Auth;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Models.Auth;
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
            return BadRequest(authTokenModelResult.Errors);
        }

        var result = _mapper.Map<SignInResponse>(authTokenModelResult.Value);
        return Ok(result);
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
