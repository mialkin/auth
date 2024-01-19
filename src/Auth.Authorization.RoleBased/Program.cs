var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication()
    .AddCookie("cookie");

services.AddAuthorization();
services.AddControllers();

var application = builder.Build();

application.UseAuthentication();
application.UseAuthorization();

application.MapControllers();

application.Run();