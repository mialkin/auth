using System.Security.Claims;
using Auth.Jwt.CookiesAndTokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var keyManager = new KeyManager();

var services = builder.Services;

services.AddSingleton(keyManager);
services.AddDbContext<IdentityDbContext>(x => x.UseInMemoryDatabase("my_database"));

services.AddIdentity<IdentityUser, IdentityRole>(x =>
    {
        x.User.RequireUniqueEmail = false;
        x.Password.RequireDigit = false;
        x.Password.RequiredLength = 4;
        x.Password.RequireLowercase = false;
        x.Password.RequireUppercase = false;
        x.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication()
    .AddJwtBearer("jwt", x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };

        x.Configuration = new OpenIdConnectConfiguration
        {
            SigningKeys =
            {
                new RsaSecurityKey(keyManager.RsaKey)
            }
        };

        x.MapInboundClaims = false;
    });

services.AddAuthorization();

var application = await builder.BuildAndSetup();

application.MapGet("/", (ClaimsPrincipal user) => user.Claims.Select(x => KeyValuePair.Create(x.Type, x.Value)));

application.MapGet("/secret", () => "Secret").RequireAuthorization("the_policy");
application.MapGet("/secret-cookie", () => "Cookie secret").RequireAuthorization("the_policy", "cookie_policy");
application.MapGet("/secret-token", () => "Token secret").RequireAuthorization("the_policy", "token_policy");

application.MapGet("/cookie/sign-in", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.PasswordSignInAsync("test@test.com", "password", false, false);
    return Results.Ok();
});


application.MapGet("/jwt/sign-in", () =>
{
    var handler = new JsonWebTokenHandler();
    var key = new RsaSecurityKey(keyManager.RsaKey);
    var token = handler.CreateToken(new SecurityTokenDescriptor
    {
        Issuer = "http://localhost:5210",
        Subject = null, // ???
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
    });

    return token;
});

application.Run();