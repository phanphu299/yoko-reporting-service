using System.Collections.Generic;
using System.Threading.Tasks;
using Reporting.Function.Model;
using Reporting.Function.Constant;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public abstract class BaseExportTrackingService : IExportTrackingService
    {
        private readonly IActivityLogMessageService _activityLogMessageService;

        protected BaseExportTrackingService(IActivityLogMessageService activityLogMessageService)
        {
            _activityLogMessageService = activityLogMessageService;
        }

        protected ICollection<TrackError> _currentErrors { get; set; }
        public ICollection<TrackError> GetErrors => _currentErrors;
        public bool HasError => (_currentErrors?.Count ?? -1) > 0;


        public virtual async Task RegisterErrorAsync(string messageCode, ErrorType errorType = ErrorType.UNDEFINED)
        {
            var message = await _activityLogMessageService.GetMessageAsync(messageCode);
            _currentErrors.Add(new TrackError(message, errorType));
        }
    }
}