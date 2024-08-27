using System;
using System.Threading.Tasks;
using Reporting.Function.Constant;
using Reporting.Function.Service.Abstraction;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Service.Abstraction;
using Function.Constants;

namespace Reporting.Function.Service
{
    public class ExportNotificationService : IExportNotificationService
    {
        public Guid ActivityId { get; set; }
        public string ObjectType { get; set; }
        public ActionType NotificationType { get; set; }
        public string Upn { get; set; }
        public string URL { get; set; }
        private string NotifyEndpoint = "ntf/notifications/export/notify";

        private readonly INotificationService _notificationService;
        public ExportNotificationService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task SendStartNotifyAsync(int count)
        {
            var message = new ExportNotifyPayload(ActivityId, ObjectType, DateTime.UtcNow, NotificationType, ActionStatus.Start)
            {
                Description = DescriptionMessage.EXPORT_START,
                Total = count
            };
            return _notificationService.SendNotifyAsync(NotifyEndpoint, new UpnNotificationMessage(ObjectType, Upn, ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID, message));
        }

        public async Task<ImportExportNotifyPayload> SendFinishExportNotifyAsync(ActionStatus status)
        {
            var message = new ExportNotifyPayload(ActivityId, ObjectType, DateTime.UtcNow, NotificationType, status)
            {
                URL = URL,
                Description = GetFinishExportNotifyDescription(status)
            };
            await _notificationService.SendNotifyAsync(NotifyEndpoint, new UpnNotificationMessage(ObjectType, Upn, ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID, message));
            return message;
        }

        private string GetFinishExportNotifyDescription(ActionStatus status)
        {
            return status switch
            {
                ActionStatus.Success => ObjectType.Equals(IOEntityType.REPORT) ? DescriptionMessage.DOWNLOAD_SUCCESS : DescriptionMessage.EXPORT_SUCCESS,
                ActionStatus.Fail => ObjectType.Equals(IOEntityType.REPORT) ? DescriptionMessage.DOWNLOAD_FAIL : DescriptionMessage.EXPORT_FAIL,
                _ => string.Empty
            };
        }
    }
}