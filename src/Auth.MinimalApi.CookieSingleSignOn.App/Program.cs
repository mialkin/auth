using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("127.0.0.1"))
    .SetApplicationName("unique");

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddAuthorization();

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "Hello from the App!");
application.MapGet("/protected", () => "Secret!").RequireAuthorization();

application.Run();