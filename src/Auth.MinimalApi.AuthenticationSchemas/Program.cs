using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddCookie("local");

services.AddAuthorization();

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "Hello world!");

application.MapGet("/login", async (HttpContext context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "aleksei"));
    var identity = new ClaimsIdentity(claims, "cookie");
    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync("local", user);

    return "ok";
});


application.Run();