using System.Text;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Authentication
{
    public static void AddAuth(this IServiceCollection service, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        service.AddSingleton(Options.Create(jwtSettings));

        service.AddTransient<IJwtService, JwtService>();

        service.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.SaveToken = true;
            opt.RequireHttpsMetadata = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });

        service.AddAuthorization(x =>
        {
            x.AddPolicy(IdentityRoles.Admin, pol =>
                pol.RequireClaim(IdentityRoles.ClaimName, IdentityRoles.Admin));
            x.AddPolicy(IdentityRoles.Customer, pol => 
                pol.RequireClaim(IdentityRoles.ClaimName, IdentityRoles.Customer));
        });
    }
}