using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using Reporting.Application.Event;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class FileEventService : IFileEventService
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;

        public FileEventService(IDomainEventDispatcher serviceProvider, ITenantContext tenantContext, IUserContext userContext)
        {
            _dispatcher = serviceProvider;
            _tenantContext = tenantContext;
            _userContext = userContext;
        }

        public Task SendExportEventAsync(Guid activityId, string objectType, IEnumerable<string> data)
        {
            var exportEvent = new FileExportEvent(activityId, objectType, data, _tenantContext, _userContext.Upn, _userContext.DateTimeFormat, _userContext.Timezone?.Offset);
            return _dispatcher.SendAsync(exportEvent);
        }

        public Task SendExportReportScheduleEventAsync(Guid activityId, string objectType, IEnumerable<string> scheduleNames, string templateId, IEnumerable<string> scheduleIds)
        {
            var exportEvent = new FileExportEvent(activityId, objectType, scheduleNames, _tenantContext, _userContext.Upn, _userContext.DateTimeFormat, _userContext.Timezone?.Offset, templateId, scheduleIds);
            return _dispatcher.SendAsync(exportEvent);
        }
    }
}