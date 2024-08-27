using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Reporting.Function.Service;
using Reporting.Function.Constant;
using Reporting.Function.Service.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.Bus.ServiceBus.Extension;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using AHI.Infrastructure.Audit.Extension;
using AHI.Infrastructure.Service.Tag.Extension;

[assembly: FunctionsStartup(typeof(Configuration.Function.Startup))]
namespace Configuration.Function
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            System.Diagnostics.Activity.DefaultIdFormat = System.Diagnostics.ActivityIdFormat.W3C;
        }

        public const string SERVICE_NAME = "reporting-function";
        public override void Configure(IFunctionsHostBuilder builder)
        {
            /* frameworks */
            builder.Services.AddRabbitMQ(SERVICE_NAME);
            builder.Services.AddMultiTenantService();

            /* DI */
            builder.Services.AddScoped<IParserContext, ParserContext>();
            builder.Services.AddScoped<INativeStorageService, NativeStorageService>();
            builder.Services.AddScoped<IFileExportService, FileExportService>();
            builder.Services.AddScoped<IExportTrackingService, ExportTrackingService>();
            builder.Services.AddScoped<IExportNotificationService, ExportNotificationService>();
            builder.Services.AddScoped<IActivityLogMessageService, ActivityLogMessageService>();
            builder.Services.AddScoped<IMasterService, MasterService>();
            builder.Services.AddScoped<IContactService, ContactService>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddEntityTagService(AHI.Infrastructure.Service.Tag.Enum.DatabaseType.SqlServer);

            builder.Services.AddScoped<NativeStorageSpaceHandler>();
            builder.Services.AddScoped<AzureBlobStorageSpaceHandler>();
            builder.Services.AddScoped<IDictionary<string, IStorageSpaceHandler>>(service =>
            {
                var dictionary = new Dictionary<string, IStorageSpaceHandler>();
                var nativeStorageSpaceHandler = service.GetRequiredService<NativeStorageSpaceHandler>();
                var azureBlobStorageSpaceHandler = service.GetRequiredService<AzureBlobStorageSpaceHandler>();
                dictionary[StorageSpace.NATIVE_STORAGE_SPACE] = nativeStorageSpaceHandler;
                dictionary[StorageSpace.AZURE_BLOB_STORAGE_SPACE] = azureBlobStorageSpaceHandler;
                return dictionary;
            });

            builder.Services.AddScoped<ReportExportHandler>();
            builder.Services.AddScoped<TemplateExportHandler>();
            builder.Services.AddScoped<ScheduleExportHandler>();

            builder.Services.AddScoped<IDictionary<string, IExportHandler>>(service =>
            {
                var dictionary = new Dictionary<string, IExportHandler>();
                var reportExportHandler = service.GetRequiredService<ReportExportHandler>();
                dictionary[IOEntityType.REPORT] = reportExportHandler;

                var templateExportHandler = service.GetRequiredService<TemplateExportHandler>();
                dictionary[IOEntityType.TEMPLATE] = templateExportHandler;

                var scheduleExportHandler = service.GetRequiredService<ScheduleExportHandler>();
                dictionary[IOEntityType.SCHEDULE] = scheduleExportHandler;


                return dictionary;
            });

            /* http */
            builder.Services.AddHttpClient(ClientName.NOTIFICATION_HUB, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                var endpoint = configuration[ConfigurationKeys.NotificationHubEndpoint];
                client.BaseAddress = new Uri(endpoint);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();
            builder.Services.AddHttpClient(ClientName.STORAGE_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.StorageServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            builder.Services.AddHttpClient(ClientName.CDN, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.CdnEndpoint]);
            });

            builder.Services.AddHttpClient(ClientName.MASTER_FUNCTION, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.MasterFunctionUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            // download link should be fully qualified URL, don't need to setup BaseAddress
            // download client must not use ClientCrendetialAuthentication handler to avoid Authorization conflict when download from blob storage
            builder.Services.AddHttpClient(ClientName.DOWNLOAD_CLIENT);

            builder.Services.AddNotification();
            builder.Services.AddAuditLogService();

            builder.Services.AddOtelTracingService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString());
            builder.Services.AddLogging(builder =>
            {
                builder.AddOpenTelemetry(option =>
               {
                   option.SetResourceBuilder(
                   ResourceBuilder.CreateDefault()
                       .AddService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString()));
                   //option.AddConsoleExporter();
                   option.AddOtlpExporter(oltp =>
                   {
                       oltp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                   });
               });
            });
        }
    }
}