using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Model;
using System;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IExportNotificationService
    {
        Guid ActivityId { get; set; }
        string ObjectType { get; set; }
        ActionType NotificationType { get; set; }
        string Upn { get; set; }
        string URL { get; set; }
        Task<ImportExportNotifyPayload> SendFinishExportNotifyAsync();
        Task SendStartExportNotifyAsync(int count);        
    }
}