using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PWP.InvoiceCapture.Core.API.Models;
using PWP.InvoiceCapture.Core.API.Services;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.Core.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string version, string serviceName) 
        {
            Guard.IsNotNull(services, nameof(services));
            Guard.IsNotNullOrWhiteSpace(version, nameof(version));
            Guard.IsNotNullOrWhiteSpace(serviceName, nameof(serviceName));

            return services.AddSwaggerGen(configuration => 
                configuration.SwaggerDoc(version,
                    new OpenApiInfo
                    {
                        Title = serviceName,
                        Version = version
                    }));
        }

        public static void AddWebApplicationContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IApplicationContext, WebApplicationContext>();
        }

        public static AuthenticationBuilder AddInvoiceCaptureAuthentication(this IServiceCollection services, InvoiceCaptureAuthenticationOptions authenticationOptions)
        {
            Guard.IsNotNull(services, nameof(services));
            Guard.IsNotNull(authenticationOptions, nameof(authenticationOptions));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidateActor = false,
                ValidateTokenReplay = false,
                IssuerSigningKey = GetSigningKey(authenticationOptions),
                ValidAudiences = authenticationOptions.ValidAudiences
            };

            return services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = !authenticationOptions.AllowInsecureHttp;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }

        public static void AddApiVersioning(this IServiceCollection services, int majorVersion = 1, int minorVersion = 0)
        {
            services.AddApiVersioning(opt => {
                opt.DefaultApiVersion = new ApiVersion(majorVersion, minorVersion);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = new HeaderApiVersionReader("apiVersion");
            });
        }

        private static SecurityKey GetSigningKey(InvoiceCaptureAuthenticationOptions options) 
        {
            Guard.IsNotNullOrWhiteSpace(options.SigningKey, nameof(options.SigningKey));

            var keyBinary = Encoding.ASCII.GetBytes(options.SigningKey);
            
            return new SymmetricSecurityKey(keyBinary);
        }
    }
}
