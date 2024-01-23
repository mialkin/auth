var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddCookie("default", options =>
    {
        options.Cookie.Name = "mycookie";
    });

services.AddControllers();

var application = builder.Build();

application.UseStaticFiles();
application.UseAuthentication();

application.MapGet("/", () => "Hello World!");

application.MapDefaultControllerRoute();

application.Run();