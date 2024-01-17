using Auth.Jwt.RoleBased.Controllers.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Jwt.RoleBased.Controllers;

[ApiController]
[Authorize] // Just require ANY authentication
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    [Route("save")]
    public IActionResult Save([FromBody] SaveUserRequest request)
    {
        return Ok("User saved at " + DateTime.Now);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [Route("delete")]
    public IActionResult Delete([FromBody] Guid id)
    {
        return Ok("User deleted at " + DateTime.Now);
    }
}