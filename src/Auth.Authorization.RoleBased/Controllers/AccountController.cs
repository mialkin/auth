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
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                        new Claim("my_role_claim_extravaganza", "admin")
                    },
                    authenticationType: "cookie",
                    nameType: null,
                    roleType: "my_role_claim_extravaganza"
                )
            ),
            authenticationScheme: "cookie"
        );
}