using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Jwt.RoleBased.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        var settings = configuration.GetRequiredSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();
        if (settings is null)
            throw new InvalidOperationException($"'{nameof(JwtTokenSettings)}' section is undefined");

        Guard.Against.NullOrEmpty(settings.SigningKey);
        Guard.Against.NullOrEmpty(settings.Audience);
        Guard.Against.NullOrEmpty(settings.Issuer);
        Guard.Against.NegativeOrZero(settings.TokenTimeoutMinutes);

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
    }
}