var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/username", (HttpContext context) =>
{
    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x != null && x.StartsWith("auth="));
    var payload = authCookie?.Split("=").Last();
    var parts = payload?.Split(":");

    var key = parts?[0];
    var value = parts?[1];

    return value;
});

app.MapGet("/login", (HttpContext context) =>
{
    context.Response.Headers["set-cookie"] = "auth=usr:aleksei";
    return "ok";
});

app.Run();