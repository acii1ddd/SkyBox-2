using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkyBox.API.Controllers;

[Authorize("Admin")]
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