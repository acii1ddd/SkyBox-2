using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SkyBox.API.Controllers;

public class BaseController : ControllerBase
{
    protected Guid AuthorizedUserId
    {
        get
        {
            var userId =  User.FindFirstValue("userId");
            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }
    }
}