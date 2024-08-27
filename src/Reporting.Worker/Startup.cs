using AHI.Infrastructure.Exception.Filter;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.MultiTenancy.Middleware;
using AHI.Infrastructure.Service.Extension;
using AHI.Infrastructure.SharedKernel;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reporting.Application.Constant;
using Reporting.Application.Service;
using Reporting.Application.Service.Abstraction;
using System;
using AHI.Infrastructure.OpenTelemetry;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Reporting.Worker.Service.Abstraction;
using Reporting.Worker.Service;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Audit.Service;

namespace Reporting.Worker
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        const string SERVICE_NAME = "reporting-worker";
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(option =>
            {
                option.ExceptionHandling();
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.NullValueHandling = Constant.JsonSerializerSetting.NullValueHandling;
                option.SerializerSettings.DateFormatString = Constant.JsonSerializerSetting.DateFormatString;
                option.SerializerSettings.ReferenceLoopHandling = Constant.JsonSerializerSetting.ReferenceLoopHandling;
                option.SerializerSettings.DateParseHandling = Constant.JsonSerializerSetting.DateParseHandling;
            });
            services.AddAuthentication()
                .AddIdentityServerAuthentication("oidc",
                jwtTokenOption =>
                {
                    jwtTokenOption.Authority = Configuration["Authentication:Authority"];
                    jwtTokenOption.RequireHttpsMetadata = Configuration["Authentication:Authority"].StartsWith("https");
                    jwtTokenOption.TokenValidationParameters.ValidateAudience = false;
                    jwtTokenOption.ClaimsIssuer = Configuration["Authentication:Issuer"];
                }, referenceTokenOption =>
                {
                    referenceTokenOption.IntrospectionEndpoint = Configuration["Authentication:IntrospectionEndpoint"];
                    referenceTokenOption.ClientId = Configuration["Authentication:ApiScopeName"];
                    referenceTokenOption.ClientSecret = Configuration["Authentication:ApiScopeSecret"];
                    referenceTokenOption.ClaimsIssuer = Configuration["Authentication:Issuer"];
                    referenceTokenOption.SaveToken = true;
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", Configuration["Authentication:ApiScopeName"]);
                });
            });

            /* DI */
            services.AddMemoryCache();
            services.AddFrameworkServices();
            services.AddLoggingService();
            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<INativeStorageService, NativeStorageService>();
            services.AddScoped<IReportBuildingService, ReportBuildingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<WorkerBackgroundService>();
            services.AddHostedService(sp => sp.GetRequiredService<WorkerBackgroundService>());

            /* http */
            services.AddHttpClient(HttpClientName.STORAGE_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                // config as public api cause the app will be deployed to app service
                client.BaseAddress = new Uri(configuration["PublicApi:Storage"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            services.AddHttpClient(HttpClientName.NOTIFICATION_HUB, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                // config as public api cause the app will be deployed to app service
                client.BaseAddress = new Uri(configuration["NotificationHubEndpoint"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            services.AddOtelTracingService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString());
            // for production, no need to output to console.
            // will adapt with open telemetry collector in the future.
            services.AddLogging(builder =>
            {
                builder.AddOpenTelemetry(option =>
               {
                   option.SetResourceBuilder(
                   ResourceBuilder.CreateDefault()
                       .AddService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString()));
                   option.AddConsoleExporter();
               });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<MultiTenancyMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                       .RequireAuthorization("ApiScope");
            });
        }
    }
}