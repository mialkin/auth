using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddCookie("default", options =>
    {
        options.Cookie.Name = "mycookie";
    });

services.AddControllers();

var application = builder.Build();

application.UseStaticFiles();
application.UseAuthentication();

application.MapGet("/", () => "Hello World!");
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
            )
        );

        return "Ok";
});

application.MapDefaultControllerRoute();

application.Run();