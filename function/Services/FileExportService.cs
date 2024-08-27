using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.Azure.WebJobs;
using Reporting.Function.Constant;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Constant;
using Newtonsoft.Json;

namespace Reporting.Function.Service
{
    public class FileExportService : IFileExportService
    {
        private readonly IExportNotificationService _notification;
        private readonly IExportTrackingService _errorService;
        private readonly IParserContext _parserContext;
        private readonly IDictionary<string, IExportHandler> _exportHandler;
        private readonly INativeStorageService _storageService;
        private readonly ILoggerAdapter<FileExportService> _logger;
        private readonly IDictionary<string, string> _logEntityType = new Dictionary<string, string>
        {
            {IOEntityType.SCHEDULE, IOEntityType.REPORT},
            {IOEntityType.TEMPLATE, IOEntityType.REPORT},
            {IOEntityType.REPORT, IOEntityType.REPORT}
        };

        public FileExportService(IExportNotificationService notificationService, IExportTrackingService errorService,
                                 IParserContext parserContext, IDictionary<string, IExportHandler> exportHandler,
                                 INativeStorageService storageService, ILoggerAdapter<FileExportService> logger)
        {
            _notification = notificationService;
            _errorService = errorService;
            _parserContext = parserContext;
            _exportHandler = exportHandler;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<ImportExportBasePayload> ExportFileAsync(string upn, Guid activityId, ExecutionContext context, string objectType, IEnumerable<string> scheduleNamesOrIds,
                                                            string dateTimeFormat, string dateTimeOffset, string templateId, IEnumerable<string> scheduleIds)
        {
            _parserContext.SetContextFormat(ContextFormatKey.DATETIMEFORMAT, dateTimeFormat);
            _parserContext.SetContextFormat(ContextFormatKey.DATETIMEOFFSET, dateTimeOffset);

            _notification.Upn = upn;
            _notification.ActivityId = activityId;
            _notification.ObjectType = _logEntityType.TryGetValue(objectType, out var notifyEntity) ? notifyEntity : objectType;
            _notification.NotificationType = ActionType.Export;

            DownloadReportRequest data = null;
            if (objectType == IOEntityType.SCHEDULE)
            {
                data = new DownloadReportRequest()
                {
                    ScheduleNames = scheduleNamesOrIds,
                    TemplateId = templateId,
                    ScheduleIds = scheduleIds,
                };
            }
            else
            {
                data = new DownloadReportRequest()
                {
                    Ids = scheduleNamesOrIds
                };
            }

            _logger.LogInformation("Send start notify");
            var count = _exportHandler.TryGetValue(objectType, out var handler) ? await handler.GetDataAsync(data) : scheduleNamesOrIds.Count();
            await _notification.SendStartNotifyAsync(count);
            try
            {
                if (handler != null)
                {
                    _logger.LogInformation("Start downloading file");

                    var downloadUrl = await handler.HandleAsync(context, data);
                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        _notification.URL = downloadUrl;
                    }
                }
                else
                    await _errorService.RegisterErrorAsync($"{objectType} not supported");
            }
            catch (System.Exception e)
            {
                await _errorService.RegisterErrorAsync(e.Message, ErrorType.DOWNLOADING);
                _logger.LogError(e, JsonConvert.SerializeObject(scheduleNamesOrIds));
            }

            _logger.LogInformation("Send finish notify");
            var status = GetFinishStatus();
            var payload = await _notification.SendFinishExportNotifyAsync(status);

            return CreateLogPayload(payload);
        }

        private ActionStatus GetFinishStatus()
        {
            return _errorService.HasError ? ActionStatus.Fail : ActionStatus.Success;
        }
        private ImportExportLogPayload<TrackError> CreateLogPayload(ImportExportNotifyPayload payload)
        {
            var detail = new[] { new ExportPayload<TrackError>((payload as ExportNotifyPayload).URL, _errorService.GetErrors) };
            return new ImportExportLogPayload<TrackError>(payload, ActionType.Download)
            {
                Detail = detail
            };
        }
    }
}