using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Authorization.RoleBased.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet("/")]
    public string Index() => "Index route";

    [Authorize(Roles = "admin")]
    [HttpGet("/secret")]
    public string Secret() => "Secrete route";
}