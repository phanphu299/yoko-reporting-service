using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Service.Abstraction;
using Reporting.Function.Constant;

namespace Reporting.Function.Service.RabbitMQ
{
    public class FileDownloadProcessing
    {
        private readonly ITenantContext _tenantContext;
        private readonly IFileExportService _fileExportService;
        private readonly IAuditLogService _auditLogService;
        private readonly IParserContext _parserContext;

        public FileDownloadProcessing(ITenantContext tenantContext, IFileExportService fileExportService, IAuditLogService auditLogService, IParserContext parserContext)
        {
            _tenantContext = tenantContext;
            _fileExportService = fileExportService;
            _auditLogService = auditLogService;
            _parserContext = parserContext;
        }

        [FunctionName("FileDownloadProcessing")]
        public async Task RunAsync(
        [RabbitMQTrigger("report.function.file.exported.processing", ConnectionStringSetting = "RabbitMQ")] byte[] data,
        ILogger log, ExecutionContext context)
        {
            BaseModel<ExportFileMessage> request = data.Deserialize<BaseModel<ExportFileMessage>>();
            var eventMessage = request.Message;

            // setup Domain to use inside repository
            _tenantContext.RetrieveFromString(eventMessage.TenantId, eventMessage.SubscriptionId, eventMessage.ProjectId);

            var result = await _fileExportService.ExportFileAsync(eventMessage.RequestedBy, eventMessage.ActivityId, context, eventMessage.ObjectType, eventMessage.Ids, eventMessage.DateTimeFormat, eventMessage.DateTimeOffset, eventMessage.TemplateId, eventMessage.ScheduleIds);
            await LogActivityAsync(result, eventMessage.RequestedBy);
        }

        private Task LogActivityAsync(ImportExportBasePayload message, string requestedBy)
        {
            var activityMessage = message.CreateLog(requestedBy, _tenantContext, _auditLogService.AppLevel);
            activityMessage.EntityId = _parserContext.GetContextFormat(ContextFormatKey.LOG_ID);
            activityMessage.EntityName = _parserContext.GetContextFormat(ContextFormatKey.LOG_NAME);
            return _auditLogService.SendLogAsync(activityMessage);
        }
    }
}