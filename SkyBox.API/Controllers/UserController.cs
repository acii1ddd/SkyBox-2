using Microsoft.AspNetCore.Mvc;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Controllers;

[Route("api/user")]
public class UserController : ControllerBase
{
    // [HttpGet]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
    // public IActionResult GetAll()
    // {
    //     return Ok(new List<User>());
    // }
}