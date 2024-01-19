using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string authScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(authScheme)
    .AddCookie(authScheme);

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/unsecure", (HttpContext ctx) => { return ctx.User.FindFirst("usr")?.Value ?? "empty"; });

app.MapGet("/sweden", (HttpContext context) =>
{
    if (context.User.Identities.All(x => x.AuthenticationType != authScheme))
    {
        context.Response.StatusCode = 401;
        return "";
    }

    if (!context.User.HasClaim("passport_type", "eur"))
    {
        context.Response.StatusCode = 403;
        return "";
    }

    return "allowed";
});

app.MapGet("/login", async (HttpContext context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "aleksei"));
    claims.Add(new Claim("passport_type", "eur"));
    var identity = new ClaimsIdentity(claims, authScheme);
    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync(authScheme, user);
});

app.Run();