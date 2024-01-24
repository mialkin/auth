using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication("default")
    .AddCookie("default", options =>
    {
        options.Cookie.Name = "mycookie";
        // options.Cookie.Domain = "mydomain";
        // options.Cookie.Path = "/test";
        // options.Cookie.HttpOnly = false;
        // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // options.Cookie.SameSite = SameSiteMode.Lax;

        // options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
        // options.SlidingExpiration = true;
    });
services.AddAuthorization();

services.AddControllers();

var application = builder.Build();

application.UseStaticFiles();
application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "Hello World!");
application.MapGet("/test", () => "Hello World!").RequireAuthorization();
application.MapPost("/login", async (HttpContext httpContext) =>
{
    await httpContext.SignInAsync(new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                },
                authenticationType: "default"
            )
        ),
        new AuthenticationProperties
        {
            IsPersistent = true
        }
    );

    return "Ok";
});

application.MapGet("/signout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync("default",
        new AuthenticationProperties
        {
            IsPersistent = true
        }
    );

    return "Ok";
});

application.MapDefaultControllerRoute();

application.Run();