using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace StockMarket.Listing.Api.Services
{
    public static class ServiceExtensions
    {
        public static void AddCustomAuth(this IServiceCollection services, IConfigurationSection config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = config.GetValue<string>("SecretKey");
                var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);
                var signingKey = new SymmetricSecurityKey(secretKeyBytes);
                var tokenParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenParams;
            });
        }

        public static void AddCustomCors(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
                options.AddPolicy(policyName, builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                )
            );
        }
    }
}