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

// This is unfinished project

services.AddAuthorization(x =>
{
    x.AddPolicy("the_policy", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
            .RequireClaim("role", "janitor");
    });
});

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

// This is unfinished project

application.MapGet("/jwt/sign-in", async (
    KeyManager manager,
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    IUserClaimsPrincipalFactory<IdentityUser> claimsPrincipalFactory) =>
{
    var user = await userManager.FindByNameAsync("test@test.com");
    if (user is null)
        throw new InvalidOperationException();

    var result = await signInManager.CheckPasswordSignInAsync(user, "password", false);

    var principal = await claimsPrincipalFactory.CreateAsync(user);
    var identity = principal.Identities.First();
    identity.AddClaim(new Claim("amr", "pwd"));
    identity.AddClaim(new Claim("method", "jwt"));

    var handler = new JsonWebTokenHandler();
    var key = new RsaSecurityKey(manager.RsaKey);
    var token = handler.CreateToken(new SecurityTokenDescriptor
    {
        Issuer = "http://localhost:5210",
        Subject = identity,
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
    });

    return token;
});

application.Run();