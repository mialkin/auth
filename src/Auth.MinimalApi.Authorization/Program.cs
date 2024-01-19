using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string authScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(authScheme)
    .AddCookie(authScheme);

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/unsecure", (HttpContext ctx) => { return ctx.User.FindFirst("usr")?.Value ?? "empty"; });

app.MapGet("/login", async (HttpContext context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "aleksei"));
    var identity = new ClaimsIdentity(claims, authScheme);
    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync(authScheme, user);
});

app.Run();