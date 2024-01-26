using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var rsa = RSA.Create();
rsa.ImportRSAPrivateKey(File.ReadAllBytes("key"), out _);

var builder = WebApplication.CreateBuilder(args);

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
                new RsaSecurityKey(rsa)
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
    var handler = new JsonWebTokenHandler();
    var key = new RsaSecurityKey(rsa);
    var token = handler.CreateToken(new SecurityTokenDescriptor
    {
        Issuer = "http://localhost:5070",
        Subject = new ClaimsIdentity(
            new[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("name", "aleksei")
            }),
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
    });

    return token;
});

application.MapGet("/jwk", () =>
{
    var publicKey = RSA.Create();
    publicKey.ImportRSAPublicKey(rsa.ExportRSAPublicKey(), out _);
    var key = new RsaSecurityKey(publicKey);
    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});


application.MapGet("/jwk-private", () =>
{
    var key = new RsaSecurityKey(rsa);
    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});

application.Run();