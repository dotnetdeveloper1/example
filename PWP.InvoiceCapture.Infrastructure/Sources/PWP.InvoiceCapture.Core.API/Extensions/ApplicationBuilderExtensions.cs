using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.Core.API.Handlers;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder applicationBuilder, string serviceName) 
        {
            Guard.IsNotNull(applicationBuilder, nameof(applicationBuilder));
            Guard.IsNotNullOrWhiteSpace(serviceName, nameof(serviceName));

            return applicationBuilder
                .UseSwagger()
                .UseSwaggerUI(configuration => configuration.SwaggerEndpoint("/swagger/v1/swagger.json", serviceName));
        }

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder applicationBuilder, IWebHostEnvironment environment)
        {
            Guard.IsNotNull(applicationBuilder, nameof(applicationBuilder));
            Guard.IsNotNull(environment, nameof(environment));

            if (environment.IsDevelopment())
            {
                return applicationBuilder.UseDeveloperExceptionPage();
            }
            
            var exceptionHandler = new GlobalExceptionHandler();

            return applicationBuilder.UseExceptionHandler(application => application.Run(exceptionHandler.InvokeAsync));
        }
    }
}
