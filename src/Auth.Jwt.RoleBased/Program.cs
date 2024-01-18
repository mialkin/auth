using Auth.Jwt.RoleBased.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.WriteTo.Console();
});

var services = builder.Services;

services.AddControllers();
services.AddRouting(options => options.LowercaseUrls = true);
services.ConfigureSwagger();
services.ConfigureAuthentication(builder.Configuration);

var application = builder.Build();

application.MapControllers();
application.UseRouting();

application.UseAuthentication();
application.UseAuthorization();

application.UseSwagger();
application.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

application.Run();