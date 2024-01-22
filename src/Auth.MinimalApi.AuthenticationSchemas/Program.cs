using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddScheme<CookieAuthenticationOptions, VisitorAuthHandler>("visitor", options => { })  // This is an authentication schema
    .AddCookie("local") // This is another authentication schema
    .AddCookie("oauth") // This is also another authentication schema
    .AddOAuth("external-oauth", options =>  // This is yet another authentication schema
    {
        options.SignInScheme = "oauth";

        options.ClientId = "id";
        options.ClientSecret = "secret";

        options.AuthorizationEndpoint = "https://oauth.wiremockapi.cloud/oauth/authorize";
        options.TokenEndpoint = "https://oauth.wiremockapi.cloud/oauth/token";
        options.UserInformationEndpoint = "https://oauth.wiremockapi.cloud/userinfo";

        options.CallbackPath = "/callback-for-auth";

        options.Scope.Add("profile");
        options.SaveTokens = true;
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("customer", policy =>
    {
        policy.AddAuthenticationSchemes("local", "visitor", "oauth")
            .RequireAuthenticatedUser();
    });

    options.AddPolicy("user", policy =>
    {
        policy.AddAuthenticationSchemes("local")
            .RequireAuthenticatedUser();
    });
});

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", (HttpContext context) =>
{
    var user = context.User;
    var identities = user.Identities;

    var stringBuilder = new StringBuilder();
    foreach (var identity in identities)
    {
        stringBuilder.AppendLine(identity.AuthenticationType);
    }

    return "Hello world!\n" + stringBuilder;
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

application.MapGet("/login-oauth", async (HttpContext context) =>
{
    await context.ChallengeAsync("external-oauth", new AuthenticationProperties
    {
        RedirectUri = "/"
    });
}).RequireAuthorization("user");


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