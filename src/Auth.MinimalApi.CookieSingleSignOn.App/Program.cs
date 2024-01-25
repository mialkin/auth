using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddAuthorization();

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "Hello from the App!");
application.MapGet("/protected", () => "Secret!").RequireAuthorization();

application.Run();