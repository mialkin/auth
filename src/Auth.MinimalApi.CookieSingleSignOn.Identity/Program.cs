using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("127.0.0.1"))
    .SetApplicationName("unique");

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        CookieAuthenticationDefaults
            .AuthenticationScheme /*, options => { options.Cookie.Domain = ".company.local";} */);

services.AddAuthorization();

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "Hello from the Identity!");
application.MapGet("/protected", () => "Secret!").RequireAuthorization();
application.MapGet("/login", (HttpContext context) =>
{
    context.SignInAsync(new ClaimsPrincipal(new[]
    {
        new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            },
            CookieAuthenticationDefaults.AuthenticationScheme
        )
    }));

    return "Ok";
});

application.Run();