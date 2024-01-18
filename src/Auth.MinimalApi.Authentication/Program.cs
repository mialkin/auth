using System.Security.Claims;
using Auth.MinimalApi.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection(); // Adds IDataProtectionProvider to IoC container
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var application = builder.Build();

application.Use((context, next) =>
{
    var provider = context.RequestServices.GetRequiredService<IDataProtectionProvider>();

    var protector = provider.CreateProtector("auth-cookie");

    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x != null && x.StartsWith("auth="));
    var protectedPayload = authCookie?.Split("=").Last();

    if (protectedPayload != null)
    {
        var payload = protector.Unprotect(protectedPayload);
        var parts = payload.Split(":");

        var key = parts[0];
        var value = parts[1];

        var claims = new List<Claim>
        {
            new(key, value)
        };

        var identity = new ClaimsIdentity(claims);
        context.User = new ClaimsPrincipal(identity);
    }

    return next();
});

application.MapGet("/username", (HttpContext context) =>
{
    return context.User.FindFirst("usr")?.Value ?? "N/A";
});

application.MapGet("/login", (AuthService authService) =>
{
    authService.SignIn();

    return "ok";
});

application.Run();