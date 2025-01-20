using Microsoft.AspNetCore.Mvc;
using SkyBox.Domain.Models;

namespace SkyBox.API.Controllers;

[Route("api/user")]
public class UserController : ControllerBase
{
    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
    public IActionResult GetAll()
    {
        return Ok(new List<User>());
    }
}