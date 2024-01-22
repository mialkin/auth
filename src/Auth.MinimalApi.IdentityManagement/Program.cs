var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<Database>();

var application = builder.Build();

application.MapGet("/", () => "Hello World!");

application.Run();