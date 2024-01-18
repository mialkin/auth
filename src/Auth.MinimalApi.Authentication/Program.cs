using Auth.MinimalApi.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection(); // Adds IDataProtectionProvider to IoC container
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var application = builder.Build();

application.MapGet("/username", (HttpContext context, IDataProtectionProvider provider) =>
{
    var protector = provider.CreateProtector("auth-cookie");

    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x != null && x.StartsWith("auth="));
    var protectedPayload = authCookie?.Split("=").Last();

    if (protectedPayload != null)
    {
        var payload = protector.Unprotect(protectedPayload);
        var parts = payload?.Split(":");

        var key = parts?[0];
        var value = parts?[1];

        return value ?? string.Empty;
    }

    return "N/A";
});

application.MapGet("/login", (AuthService authService) =>
{
    authService.SignIn();

    return "ok";
});

application.Run();