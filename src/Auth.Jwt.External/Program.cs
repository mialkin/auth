using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwkString = "{\"e\":\"AQAB\",\"key_ops\":[],\"kty\":\"RSA\",\"n\":\"1iNWne5sUn1MoQQ2lpVM3VJrHlXRd89sAonBVnkHzyJj6KIOfcTFfyWh6uqk1AyAzInkkngivZFKlqRpG66bLammIVcZ9iXET27XB09TUOkcFRBybuDUSNNriEl5_Xr1j6OqD_pVk7hq1Nue1kDW77YjHoPHBWc-I_QpKgVox1EeOIAMP9PwJhuprrQ4M07lg58Sa8XLNUI0X5aBp_29EGFFqtvs7IIGDC_ngNUyW8eKa2Rv7kWY2dY8dcRMwuf2mHeTNwm8RuCGJR2j6LBr7392Qm6a9w6K6UE93veHOlx9vIUJcxUA5iIMRi6D3aCugiwF1CQAM9ftuXlBPzwSAQ\",\"oth\":[],\"x5c\":[]}";

var services = builder.Services;

services.AddAuthentication()
    .AddJwtBearer("jwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.ContainsKey("t"))
                {
                    context.Token = context.Request.Query["t"];
                }

                return Task.CompletedTask;
            }
        };

        options.Configuration = new OpenIdConnectConfiguration
        {
            SigningKeys =
            {
                JsonWebKey.Create(jwkString)
            }
        };

        options.MapInboundClaims = false;
    });

var application = builder.Build();

application.UseAuthentication();

application.MapGet("/", (HttpContext context) =>
{
    var claim = context.User.FindFirst("sub")?.Value ?? "Empty";

    return claim;
});

application.MapGet("/jwt", () => 
{
    // This endpoint will fail which proves that you can't generate a JWT key with public RSA key.
    // But you can verify JWT key with public RSA key.
    // To make this endpoint work provide private RSA key instead of private key
    
    var handler = new JsonWebTokenHandler();
    var token = handler.CreateToken(new SecurityTokenDescriptor
    {
        Issuer = "http://localhost:5070",
        Subject = new ClaimsIdentity(
            new[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("name", "aleksei")
            }),
        SigningCredentials = new SigningCredentials(JsonWebKey.Create(jwkString), SecurityAlgorithms.RsaSha256)
    });

    return token;
});

application.Run();