using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Simple.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        var claim = new Claim(type: ClaimTypes.Role, value: "admin");
        var claimIdentity = new ClaimsIdentity(claims: new[] { claim }, authenticationType: "login");
        var claimsPrincipal = new ClaimsPrincipal(identities: new[] { claimIdentity });

        await HttpContext.SignInAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            principal: claimsPrincipal);

        return Ok();
    }

    [Authorize]
    [HttpGet("get-user-info")]
    public IActionResult GetUserInfo()
    {
        var user = User;
        var response = new
        {
            user.Identity?.IsAuthenticated,
            user.Identity?.AuthenticationType,
            user.Claims.First().Value
        };
        
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        
        return Ok();
    }
}