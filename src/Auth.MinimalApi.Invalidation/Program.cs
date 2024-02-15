using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");

var application = builder.Build();

application.MapGet("/login", () => Results.SignIn(
        new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                },
                authenticationType: "cookie"
            )
        ),
        new AuthenticationProperties(),
        "cookie"
    )
);

application.MapGet("/user", (ClaimsPrincipal user) => user.Claims.Select(x => new { x.Type, x.Value }).ToList());

application.Run();