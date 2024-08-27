using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reporting.Application.Command;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class ReportingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILoggerAdapter<ReportingBackgroundService> _logger;

        public ReportingBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration, IBackgroundTaskQueue taskQueue, ILoggerAdapter<ReportingBackgroundService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var queueMessage = await _taskQueue.DequeueAsync(stoppingToken);

                // TODO:
                // Propose: Enhance running without await here, since await one task will block other task in queue
                // Caution: one generate report task (for ReportAndSend Type) already run multiple tasks in parallel to generate multiple templates,
                //          should check performance and have a limit mechanism to limit number of task run
                // Other alternative: Enhance using multiple queues (with multiple background services?) to separate different type of work,
                //                    so task of one type don't have to wait tasks of other types.
                // Same Caution as above proposal
                try
                {
                    switch (queueMessage.Command)
                    {
                        case TriggerGenerateReport generateReportCommand:
                            await GenerateReportAsync(generateReportCommand, queueMessage.TenantContext, queueMessage.UserContext);
                            break;
                        case ExportPreviewReport exportPreviewCommand:
                            await CreatePreviewReportBackgroundAsync(exportPreviewCommand, queueMessage.TenantContext, queueMessage.UserContext);
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

        public async Task CreatePreviewReportBackgroundAsync(ExportPreviewReport command, ITenantContext tenantContext, IUserContext userContext)
        {

            using (var scope = _serviceProvider.CreateScope(tenantContext, userContext))
            {
                var scopeTemplateService = scope.ServiceProvider.GetService(typeof(ITemplateService)) as ITemplateService;
                var auditLogService = scope.ServiceProvider.GetService(typeof(IAuditLogService)) as IAuditLogService;
                var scopeNotificationService = scope.ServiceProvider.GetService(typeof(IExportNotificationService)) as IExportNotificationService;

                var commandGetTemplate = new GetTemplateById(command.TemplateId);
                var template = await scopeTemplateService.GetTemplateByIdAsync(commandGetTemplate);
                var timeZoneName = userContext?.Timezone?.Name ?? DateTimeExtension.DEFAULT_TIMEZONE_NAME;

                var reportService = scope.ServiceProvider.GetService(typeof(IReportService)) as IReportService;

                try
                {
                    var reportFileUrl = await reportService.CreatePreviewReportAsync(command, template, timeZoneName);
                    if (string.IsNullOrWhiteSpace(reportFileUrl))
                    {
                        return;
                    }

                    scopeNotificationService.Upn = userContext.Upn;
                    scopeNotificationService.URL = reportFileUrl;
                    scopeNotificationService.ActivityId = command.ActivityId;
                    scopeNotificationService.ObjectType = ObjectType.REPORT_OBJECT_TYPE;
                    scopeNotificationService.NotificationType = ActionType.Export;

                    await scopeNotificationService.SendFinishExportNotifyAsync();

                    var fullPathReportFileUrl = $"{_configuration["PublicApi:Storage"]}/{reportFileUrl.TrimStart('/')}";
                    await auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Download, ActionStatus.Success, template.Id, template.Name, new { ReportFileUrl = fullPathReportFileUrl, TemplateInfo = template });
                }
                catch (System.Exception ex)
                {
                    await auditLogService.SendLogAsync(ActivityLogEntityConstants.TEMPLATE, ActionType.Download, ex, template.Id, template.Name, command);
                    throw;
                }
            }
        }

        public async Task GenerateReportAsync(TriggerGenerateReport command, ITenantContext tenantContext, IUserContext userContext)
        {
            using (var scope = _serviceProvider.CreateScope(tenantContext, userContext))
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var request = new TriggerExecution(command.ScheduleExecutionId, command.RetryJobId);
                await mediator.Send(request);
            }
        }
    }
}