using AHI.Infrastructure.Audit.Extension;
using AHI.Infrastructure.Bus.ServiceBus.Extension;
using AHI.Infrastructure.Cache.Redis.Extension;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.OpenTelemetry;
using AHI.Infrastructure.Service.Extension;
using AHI.Infrastructure.Service.Tag.Enum;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.UserContext.Extension;
using Device.Application.Constant;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Prometheus;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Schedule.Validation;
using Reporting.Application.Service;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Services;
using Reporting.Application.Services.Abstractions;
using Reporting.Application.Template.Validation;
using Reporting.Application.Validation;
using Reporting.Pipeline;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reporting.Application.Extension
{
    public static class ApplicationExtension
    {
        const string SERVICE_NAME = "reporting-service";
        public static void AddApplicationService(this IServiceCollection serviceCollection, bool isDevelopment = true)
        {
            /* DI framework */
            serviceCollection.AddApplicationValidator();
            serviceCollection.AddFrameworkServices();
            serviceCollection.AddUserContextService();
            serviceCollection.AddRedisCache();
            serviceCollection.AddRabbitMQ(SERVICE_NAME);
            serviceCollection.AddMediatR(typeof(ApplicationExtension).GetTypeInfo().Assembly);
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            serviceCollection.AddScoped<IRequestContext, RequestContext>();
            serviceCollection.AddScoped<ISystemContext, SystemContext>();
            serviceCollection.AddEntityTagService(DatabaseType.SqlServer);

            /* DI internal */
            serviceCollection.AddScoped<ISchemaService, SchemaService>();
            serviceCollection.AddScoped<ISchemaValidator, SchemaValidator>();
            serviceCollection.AddScoped<IStorageTypeService, StorageTypeService>();
            serviceCollection.AddScoped<IOutputTypeService, OutputTypeService>();
            serviceCollection.AddScoped<IDataSourceTypeService, DataSourceTypeService>();
            serviceCollection.AddScoped<IStorageService, StorageService>();
            serviceCollection.AddScoped<ITemplateService, TemplateService>();
            serviceCollection.AddScoped<IReportService, ReportService>();
            serviceCollection.AddScoped<IReportTemplateService, ReportTemplateService>();
            serviceCollection.AddScoped<IReportScheduleService, ReportScheduleService>();
            serviceCollection.AddScoped<INativeStorageService, NativeStorageService>();
            serviceCollection.AddScoped<IFileEventService, FileEventService>();
            serviceCollection.AddScoped<IJobService, JobService>();
            serviceCollection.AddScoped<IScheduleService, ScheduleService>();
            serviceCollection.AddScoped<IWorkerService, WorkerService>();
            serviceCollection.AddScoped<IScheduleTypeService, ScheduleTypeService>();
            serviceCollection.AddScoped<IScheduleExecutionService, ScheduleExecutionService>();
            serviceCollection.AddScoped<IExportNotificationService, ExportNotificationService>();
            serviceCollection.AddScoped<IFailedScheduleService, FailedScheduleService>();
            serviceCollection.AddScoped<IUserService, UserService>();

            serviceCollection.AddSingleton<RetryOptions>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var retryOptions = new RetryOptions();
                configuration.GetSection("Retry").Bind(retryOptions);
                return retryOptions;
            });

            serviceCollection.AddSingleton<WhatsAppTemplateOptions>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var whatsappTemplateOptions = new WhatsAppTemplateOptions();
                configuration.GetSection("WhatsAppTemplates").Bind(whatsappTemplateOptions);
                return whatsappTemplateOptions;
            });

            serviceCollection.AddScoped<ReportGenerator>();
            serviceCollection.AddScoped<IReportProcessor, ReportAndSendProcessor>();

            serviceCollection.AddScoped<INotificationHandler, NotificationHandler>();
            serviceCollection.AddScoped<IReportNotifyContentBuilder, ReportNotifyContentBuilder>();

            serviceCollection.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            serviceCollection.AddSingleton<ReportingBackgroundService>();
            serviceCollection.AddHostedService(sp => sp.GetRequiredService<ReportingBackgroundService>());

            serviceCollection.AddSingleton<ICommandQueue<CollectReports>, CommandQueue<CollectReports>>();
            // TODO: repeat this line to add more workers
            serviceCollection.AddSingleton<IHostedService, BackgroundCommandProcessor<CollectReports>>();

            serviceCollection.AddScoped<IRequestDataManager, RequestDataManager>();
            serviceCollection.AddAuditLogService();
            serviceCollection.AddNotification();

            serviceCollection.AddScoped<ApiDataSourceHandler>();
            serviceCollection.AddScoped<SqlServerDataSourceHandler>();
            serviceCollection.AddScoped<PostgreSQLDataSourceHandler>();
            serviceCollection.AddScoped<AmDataSourceHandler>();
            serviceCollection.AddScoped<IDictionary<string, IDataSourceHandler>>(service =>
            {
                var dictionary = new Dictionary<string, IDataSourceHandler>();
                var apiDataSourceHandler = service.GetRequiredService<ApiDataSourceHandler>();
                var sqlServerDataSourceHandler = service.GetRequiredService<SqlServerDataSourceHandler>();
                var postgreDataSourceHandler = service.GetRequiredService<PostgreSQLDataSourceHandler>();
                var amDataSourceHandler = service.GetRequiredService<AmDataSourceHandler>();
                dictionary[DataSource.API_DATA_SOURCE] = apiDataSourceHandler;
                dictionary[DataSource.SQL_SERVER_DATA_SOURCE] = sqlServerDataSourceHandler;
                dictionary[DataSource.POSTGRE_DATA_SOURCE] = postgreDataSourceHandler;
                dictionary[DataSource.ASSET_MANAGEMENT_DATA_SOURCE] = amDataSourceHandler;
                return dictionary;
            });

            serviceCollection.AddScoped<NativeStorageSpaceHandler>();
            serviceCollection.AddScoped<AzureBlobStorageSpaceHandler>();
            serviceCollection.AddScoped<IDictionary<string, IStorageSpaceHandler>>(service =>
            {
                var dictionary = new Dictionary<string, IStorageSpaceHandler>();
                var nativeStorageSpaceHandler = service.GetRequiredService<NativeStorageSpaceHandler>();
                var azureBlobStorageSpaceHandler = service.GetRequiredService<AzureBlobStorageSpaceHandler>();
                dictionary[StorageSpace.NATIVE_STORAGE_SPACE] = nativeStorageSpaceHandler;
                dictionary[StorageSpace.AZURE_BLOB_STORAGE_SPACE] = azureBlobStorageSpaceHandler;
                return dictionary;
            });

            serviceCollection.AddScoped<SendScheduleValidationHandler>();
            serviceCollection.AddScoped<ReportAndSendScheduleValidationHandler>();
            serviceCollection.AddScoped<IDictionary<string, IScheduleValidationHandler>>(service =>
            {
                var dictionary = new Dictionary<string, IScheduleValidationHandler>();
                var sendValidationHandler = service.GetRequiredService<SendScheduleValidationHandler>();
                var reportAndSendValidationHandler = service.GetRequiredService<ReportAndSendScheduleValidationHandler>();
                dictionary[ScheduleType.SEND] = sendValidationHandler;
                dictionary[ScheduleType.REPORT_AND_SEND] = reportAndSendValidationHandler;
                return dictionary;
            });

            /* http */
            serviceCollection.AddHttpClient(HttpClientName.STORAGE_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                // config as public api cause the app will be deployed to app service
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.StorageServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.REPORTING_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                // config as public api cause the app will be deployed to app service
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.ReportingServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.DEVICE_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.DeviceServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.SCHEDULER_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.SchedulerServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.TENANT_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.TenantServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.PROJECT_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.ProjectServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.CONFIGURATION_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.ConfigurationServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.USER_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.UserServiceUrl]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();
            serviceCollection.AddHttpClient(HttpClientName.CDN, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration[ConfigurationKeys.CdnEndpoint]);
            }).UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClientName.TAG, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Api:Tag"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            // download link should be fully qualified URL, don't need to setup BaseAddress
            // download client must not use ClientCrendetialAuthentication handler to avoid Authorization conflict when download from blob storage
            serviceCollection.AddHttpClient(HttpClientName.DOWNLOAD_CLIENT);

            serviceCollection.AddOtelTracingService(SERVICE_NAME, typeof(ApplicationExtension).Assembly.GetName().Version.ToString());
            // for production, no need to output to console.
            // will adapt with open telemetry collector in the future.
            serviceCollection.AddLogging(builder =>
            {
                builder.AddOpenTelemetry(option =>
               {
                   option.SetResourceBuilder(
                   ResourceBuilder.CreateDefault()
                       .AddService(SERVICE_NAME, typeof(ApplicationExtension).Assembly.GetName().Version.ToString()));
                   option.AddOtlpExporter(oltp =>
                   {
                       oltp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                   });
               });
            });
        }

        public static void AddApplicationValidator(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<FluentValidation.IValidator<AddStorage>, AddStorageValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<UpdateStorage>, UpdateStorageValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<DeleteStorage>, DeleteStorageValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<AddTemplate>, AddTemplateValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<UpdateTemplate>, UpdateTemplateValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<DeleteTemplate>, DeleteTemplateValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<PreviewReport>, PreviewReportValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<GenerateReport>, GenerateReportValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<ExportReport>, ExportReportValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<AddSchedule>, AddScheduleValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<UpdateSchedule>, UpdateScheduleValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<DeleteSchedule>, DeleteScheduleValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<GetDatasetFromTemplateFile>, GetDatasetFromTemplateFileValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<ScheduleDto>, ArchiveScheduleValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<TemplateDto>, ArchiveTemplateDtoValidation>();
        }
    }
}