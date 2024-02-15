using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var blackList = new List<string>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", options =>
    {
        options.Events.OnValidatePrincipal = context =>
        {
            // context.HttpContext.RequestServices.GetRequiredService<IBlackListStorage>();
            if (blackList.Contains(context.Principal!.FindFirstValue("session")!))
            {
                context.RejectPrincipal();
            }

            return Task.CompletedTask;
        };
    });

var application = builder.Build();

application.MapGet("/login", () => Results.SignIn(
        new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim("session", Guid.NewGuid().ToString()),
                },
                authenticationType: "cookie"
            )
        ),
        new AuthenticationProperties(),
        "cookie"
    )
);

application.MapGet("/user", (ClaimsPrincipal user) => user.Claims.Select(x => new { x.Type, x.Value }).ToList());

// http://localhost:5170/blacklist?session=33136d12-b6b8-4063-b7f7-071330d15f51
application.MapGet("/blacklist", (string session) => { blackList.Add(session); });

application.Run();