using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string authScheme = "cookie";
const string authScheme2 = "cookie2";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(authScheme)
    .AddCookie(authScheme)
    .AddCookie(authScheme2);

var application = builder.Build();

application.UseAuthentication();

application.Use((context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/login"))
    {
        return next();
    }
    
    if (context.User.Identities.All(x => x.AuthenticationType != authScheme))
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    }
    
    if (!context.User.HasClaim("passport_type", "eur"))
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    }
    
    return next();
});

application.MapGet("/unsecure", (HttpContext ctx) => { return ctx.User.FindFirst("usr")?.Value ?? "empty"; });

application.MapGet("/sweden", (HttpContext context) =>
{
    // if (context.User.Identities.All(x => x.AuthenticationType != authScheme))
    // {
    //     context.Response.StatusCode = 401;
    //     return "";
    // }
    //
    // if (!context.User.HasClaim("passport_type", "eur"))
    // {
    //     context.Response.StatusCode = 403;
    //     return "";
    // }

    return "allowed";
});

application.MapGet("/norway", (HttpContext context) =>
{
    // if (context.User.Identities.All(x => x.AuthenticationType != authScheme))
    // {
    //     context.Response.StatusCode = 401;
    //     return "";
    // }
    //
    // if (!context.User.HasClaim("passport_type", "NOR"))
    // {
    //     context.Response.StatusCode = 403;
    //     return "";
    // }

    return "allowed";
});

//[AuthScheme(authScheme2)]
// [AuthClaim("passport_type", "eur")]
application.MapGet("/denmark", (HttpContext context) =>
{
    if (context.User.Identities.All(x => x.AuthenticationType != authScheme2))
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

application.MapGet("/login", async (HttpContext context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "aleksei"));
    claims.Add(new Claim("passport_type", "eur"));
    var identity = new ClaimsIdentity(claims, authScheme);
    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync(authScheme, user);
});

application.Run();