using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Auth.MinimalApi.IdentityManagement;

public class UserHelper
{
    public static ClaimsPrincipal Convert(User user)
    {
        var claims = new List<Claim>
        {
            new("username", user.Username)
        };

        claims.AddRange(user.Claims.Select(x => new Claim(x.Type, x.Value)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identity);
    }
}