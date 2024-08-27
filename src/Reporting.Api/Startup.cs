using AHI.Infrastructure.Exception.Filter;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Middleware;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.UserContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reporting.Application.Extension;
using Reporting.Persistence.Extension;
using Prometheus;
using Prometheus.SystemMetrics;

namespace Reporting.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationService(Environment.IsDevelopment());
            services.AddPersistenceService(Environment.IsDevelopment());
            services.AddMultiTenantService();
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
            
            services.AddSystemMetrics();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseHttpMetrics();
            app.UseAuthorization();
            app.UseWhen(
                context => !(context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/metrics")),
                builder =>
                {
                    builder.UseMiddleware<MultiTenancyMiddleware>();
                    builder.UseMiddleware<UserContextMiddleware>();});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                       .RequireAuthorization("ApiScope");
            });
        }
    }
}