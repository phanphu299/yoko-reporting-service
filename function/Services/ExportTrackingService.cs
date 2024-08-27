using System.Collections.Generic;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class ExportTrackingService : BaseExportTrackingService
    {
        public ExportTrackingService(IActivityLogMessageService activityLogMessageService) : base(activityLogMessageService)
        {
            _currentErrors = new List<TrackError>();
        }
    }
}