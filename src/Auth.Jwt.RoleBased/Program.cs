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

var settings = new JwtTokenSettings
{
    Issuer = "https://mysite.com",
    Audience = "https://mysite.com",
    SigningKey = "tLoezSkU6WtUlyb5kXS+vrx5Q51+QoF1l6BuP3Lw2t6K2+VhaO8NqbLkh1/QbYbLoh0DvOEGbrMQsxmdGPrMOSf5w6bP8FFEfNsc"

};

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

var application = builder.Build();

application.UseRouting();

application.UseAuthentication();
application.UseAuthorization();

application.UseStatusCodePages(); // TODO Remove?

application.MapControllers();

application.Run();