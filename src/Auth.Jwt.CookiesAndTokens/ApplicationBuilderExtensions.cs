using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Auth.Jwt.CookiesAndTokens;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> BuildAndSetup(this WebApplicationBuilder builder)
    {
        var application = builder.Build();

        using (var scope = application.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser { UserName = "test@test.com", Email = "test@test.com" };
            await userManager.CreateAsync(user, password: "password");
            await userManager.AddClaimAsync(user, new Claim("role", "janitor"));
        }

        return application;
    }
}