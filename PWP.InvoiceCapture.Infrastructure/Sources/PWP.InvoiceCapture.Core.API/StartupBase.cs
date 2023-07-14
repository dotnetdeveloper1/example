using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.Core.API.Extensions;
using PWP.InvoiceCapture.Core.API.Filters;
using PWP.InvoiceCapture.Core.API.Models;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.API
{
    public abstract class StartupBase
    {
        public StartupBase(IWebHostEnvironment environment)
        {
            Guard.IsNotNull(environment, nameof(environment));

            var builder = new ConfigurationBuilder()
               .SetBasePath(environment.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
               .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            AddAuthentication(services);
            services.AddMvc(ConfigureMvc);
            services.AddCors();
            services.AddOptions();
            services.AddSwagger(apiVersion, serviceName);
            services.AddApplicationInsightsTelemetry(configuration);
            services.AddControllers();
            services.AddWebApplicationContext();
            services.AddHealthChecks();

            ConfigureOptions(services);
            ConfigureModules(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder application, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment environment)
        {
            Guard.IsNotNull(application, nameof(application));
            Guard.IsNotNull(applicationLifetime, nameof(applicationLifetime));
            Guard.IsNotNull(environment, nameof(environment));

            applicationLifetime.ApplicationStopped.Register(OnShutdown);

            application.UseGlobalExceptionHandler(environment);

            application.UseCors(corsPolicyBuilder => 
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            application.UseSwagger(serviceName);
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthCheck");
            });
        }

        protected abstract string serviceName { get; }

        protected abstract string apiVersion { get; }

        protected abstract ICompositionModule[] GetCompositionModules();

        protected virtual void ConfigureMvc(MvcOptions options)
        {
            GlobalAuthorizationFilter.Register(options);
        }

        protected virtual void AddAuthentication(IServiceCollection services)
        {
            var authenticationOptions = new InvoiceCaptureAuthenticationOptions();
            configuration.GetSection("AuthenticationOptions").Bind(authenticationOptions);

            services.AddInvoiceCaptureAuthentication(authenticationOptions);
        }

        protected virtual void ConfigureOptions(IServiceCollection services)
        { }

        protected virtual void OnShutdown()
        { }

        protected readonly IConfiguration configuration;

        private void ConfigureModules(IServiceCollection services)
        {
            var modules = GetCompositionModules();

            foreach (var module in modules)
            {
                module.RegisterTypes(services);
            }
        }
    }
}
