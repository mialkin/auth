using System.Text;
using Auth.Jwt.RoleBased;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

var settings = builder.Configuration.GetRequiredSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();
if (settings is null)
    throw new InvalidOperationException();

services.AddAuthentication(auth =>
    {
        auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey))
        };
    });

services.AddOptions<JwtTokenSettings>().BindConfiguration(nameof(JwtTokenSettings));

var application = builder.Build();

application.UseRouting();

application.UseAuthentication();
application.UseAuthorization();

application.UseStatusCodePages(); // TODO Remove?

application.MapControllers();

application.Run();