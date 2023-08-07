using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.Models;

namespace Simple.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(Login model)
    {
        var claim = new Claim(ClaimTypes.Role, "admin");
        var claimIdentity = new ClaimsIdentity(new[] { claim }, "login");
        var claimsPrincipal = new ClaimsPrincipal(new[] { claimIdentity });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Ok();
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        ClaimsPrincipal user = User;

        await HttpContext.SignOutAsync();
        return Ok("Un");
    }
}