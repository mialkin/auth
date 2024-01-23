using Auth.MinimalApi.IdentityManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddAuthorization(options =>
{
    options.AddPolicy("manager", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireClaim("role", "manager");
    });
});

services.AddSingleton<Database>();
services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

var application = builder.Build();

application.UseAuthentication();

application.MapGet("/", () => "Hello world!");
application.MapGet("/protected", () => "Something super secret").RequireAuthorization("manager");

application.MapGet("/register", async (
    string username,
    string password,
    IPasswordHasher<User> hasher,
    Database database,
    HttpContext context
) =>
{
    var user = new User(username);
    user.PasswordHash = hasher.HashPassword(user, password);
    await database.PutAsync(user);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, UserHelper.Convert(user));

    return user;
});

application.MapGet("/login", async (
    string username,
    string password,
    IPasswordHasher<User> hasher,
    Database database,
    HttpContext context
) =>
{
    var user = await database.GetUserAsync(username);
    if (user is null)
        return Results.NotFound();

    if (user.PasswordHash is null)
        return Results.BadRequest();

    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
    if (result == PasswordVerificationResult.Failed)
    {
        return Results.BadRequest("Bad credentials");
    }

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, UserHelper.Convert(user));

    return Results.Ok("Logged in successfully");
});

application.MapGet("/promote", async (
    string username,
    Database database
) =>
{
    var user = await database.GetUserAsync(username);
    if (user is null)
        return Results.NotFound();

    user.Claims.Add(new UserClaim("role", "manager"));
    await database.PutAsync(user);

    return Results.Ok("Promoted");
});

application.Run();