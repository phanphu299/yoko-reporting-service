using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Service;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Device.Application.Constant;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reporting.Application.Command;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using Reporting.Worker.Service.Abstraction;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class WorkerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Channel<QueueMessage> _channel;
        private readonly ILoggerAdapter<WorkerBackgroundService> _logger;
        private readonly IMemoryCache _memoryCache;

        public WorkerBackgroundService(IServiceProvider serviceProvider, ILoggerAdapter<WorkerBackgroundService> logger, IMemoryCache memoryCache)
        {
            _channel = Channel.CreateUnbounded<QueueMessage>();
            _logger = logger;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var queueMessage = await _channel.Reader.ReadAsync(stoppingToken);
                try
                {
                    switch (queueMessage.Command)
                    {
                        case PreviewReportFile command:
                            await PreviewReportFileAsync(command);
                            break;

                        default:
                            break;
                    }
                }
                catch (System.Exception exc)
                {
                    _logger.LogError(exc, exc.Message);
                }
            }
        }

        public async Task PreviewReportFileAsync(PreviewReportFile command)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tenantContext = scope.ServiceProvider.GetService(typeof(ITenantContext)) as ITenantContext;

                tenantContext.SetTenantId(command.TenantId);
                tenantContext.SetSubscriptionId(command.SubscriptionId);
                tenantContext.SetProjectId(command.ProjectId);

                var reportBuildingService = scope.ServiceProvider.GetService(typeof(IReportBuildingService)) as IReportBuildingService;
                var storageService = scope.ServiceProvider.GetService(typeof(INativeStorageService)) as INativeStorageService;
                var notificationService = scope.ServiceProvider.GetService(typeof(INotificationService)) as NotificationService;

                try
                {
                    var previewKey = command.GetPreviewkey();
                    var fileUrl = _memoryCache.Get<string>(previewKey);

                    if (string.IsNullOrEmpty(fileUrl))
                    {
                        var filledStream = await reportBuildingService.BuildReportFileAsync(command);
                        var commandUploadStorage = new UploadStorageSpace(command.Template, filledStream, command.TimeZoneName, DateTime.UtcNow);
                        fileUrl = await storageService.UploadAsync(commandUploadStorage.FileName, commandUploadStorage.FolderName, commandUploadStorage.File);
                        _memoryCache.Set(previewKey, fileUrl);
                    }

                    var message = new { ObjectType = ObjectType.PREVIEW_REPORT_OBJECT_TYPE, ActivityId = command.ActivityId, URL = fileUrl };
                    await notificationService.SendNotifyAsync("ntf/notifications/export/notify", new UpnNotificationMessage(ObjectType.PREVIEW_REPORT_OBJECT_TYPE, command.Upn, applicationId: ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID, message));
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        public async Task QueueAsync(object command)
        {
            if (command == null)
            {
                throw new EntityInvalidException(nameof(command));
            }
            await _channel.Writer.WriteAsync(new QueueMessage(command));
        }

        private class QueueMessage
        {
            public object Command { get; set; }

            public QueueMessage(object command)
            {
                Command = command;
            }
        }
    }
}