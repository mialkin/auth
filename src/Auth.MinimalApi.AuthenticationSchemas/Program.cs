using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddScheme<CookieAuthenticationOptions, VisitorAuthHandler>("visitor", options => { })
    .AddCookie("local"); // This is also another authentication schema

services.AddAuthorization(options =>
{
    options.AddPolicy("customer", policy =>
    {
        policy.AddAuthenticationSchemes("local", "visitor")
            .RequireAuthenticatedUser();
    });
});

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", (HttpContext context) =>
{
    var user = context.User;
    var identites = user.Identities;

    return "Hello world!";
}).RequireAuthorization("customer");

application.MapGet("/login-local", async (HttpContext context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "aleksei"));
    var identity = new ClaimsIdentity(claims, "local");
    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync("local", user);

    return "ok";
});

application.Run();

public class VisitorAuthHandler : CookieAuthenticationHandler
{
    public VisitorAuthHandler(
        IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();
        if (result.Succeeded)
        {
            return result;
        }

        var claims = new List<Claim>();
        claims.Add(new Claim("usr", "aleksei"));
        var identity = new ClaimsIdentity(claims, "visitor");
        var user = new ClaimsPrincipal(identity);

        await Context.SignInAsync("visitor", user);

        return AuthenticateResult.Success(new AuthenticationTicket(user, "visitor"));
    }
}