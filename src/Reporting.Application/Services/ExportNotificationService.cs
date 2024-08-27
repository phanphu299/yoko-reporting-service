using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Model;
using Reporting.Application.Service.Abstraction;
using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Service.Abstraction;
using Device.Application.Constant;

namespace Reporting.Application.Service
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

        public async Task<ImportExportNotifyPayload> SendFinishExportNotifyAsync()
        {
            var message = new ExportNotifyPayload(ActivityId, ObjectType, DateTime.UtcNow, NotificationType, ActionStatus.Success)
            {
                URL = URL,
                Description = DescriptionMessage.DOWNLOAD_SUCCESS,
            };
            await _notificationService.SendNotifyAsync(NotifyEndpoint, new UpnNotificationMessage(ObjectType, Upn, ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID, message));
            return message;
        }

        public Task SendStartExportNotifyAsync(int count)
        {
            var message = new ExportNotifyPayload(ActivityId, ObjectType, DateTime.UtcNow, NotificationType, ActionStatus.Start)
            {
                Description = DescriptionMessage.EXPORT_START,
                Total = count
            };
            return _notificationService.SendNotifyAsync(NotifyEndpoint, new UpnNotificationMessage(ObjectType, Upn, ApplicationInformation.ASSET_DASHBOARD_APPLICATION_ID, message));
        }
    }
}