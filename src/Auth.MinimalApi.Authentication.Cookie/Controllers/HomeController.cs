using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Auth.MinimalApi.Authentication.Cookie.Controllers;

public class HomeController : Controller
{
    [HttpPost("mvc/login")]
    public async Task<IActionResult> Login()
    {
        await HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                    },
                    authenticationType: "default"
                )
            )
        );

        return Ok();
    }
}