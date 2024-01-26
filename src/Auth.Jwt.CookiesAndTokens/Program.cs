using System.Security.Claims;
using Auth.Jwt.CookiesAndTokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var keyManager = new KeyManager();

var application = builder.Build();

application.MapGet("/", () => "Hello World!");

application.MapGet("/jwt", () =>
{
    var handler = new JsonWebTokenHandler();
    var key = new RsaSecurityKey(keyManager.RsaKey);
    var token = handler.CreateToken(new SecurityTokenDescriptor
    {
        Issuer = "http://localhost:5210",
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

application.Run();