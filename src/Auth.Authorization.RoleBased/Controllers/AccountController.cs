using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Authorization.RoleBased.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpGet("/login")]
    public IActionResult Login() =>
        SignIn(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                    },
                    authenticationType: "cookie"
                )
            ),
            authenticationScheme: "cookie"
        );
}