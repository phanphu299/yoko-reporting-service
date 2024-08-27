using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Event
{
    public class FileExportEvent : BusEvent
    {
        public override string TopicName => "report.application.event.file.exported";
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public string ObjectType { get; set; }
        public IEnumerable<string> Ids { get; set; }
        public string TemplateId { get; set; }
        public IEnumerable<string> ScheduleIds { get; set; }
        public string RequestedBy { get; set; }
        public string DateTimeFormat { get; set; }
        public string DateTimeOffset { get; set; }

        public FileExportEvent(Guid activityId, string objectType, IEnumerable<string> ids,
            ITenantContext tenantContext, string requestedBy,
            string dateTimeFormat = null, string dateTimeOffset = null, string templateId = null, IEnumerable<string> scheduleIds = null)
        {
            ActivityId = activityId;
            ObjectType = objectType;
            TenantId = tenantContext.TenantId;
            SubscriptionId = tenantContext.SubscriptionId;
            ProjectId = tenantContext.ProjectId;
            Ids = ids;
            RequestedBy = requestedBy;
            DateTimeFormat = dateTimeFormat;
            DateTimeOffset = dateTimeOffset;
            TemplateId = templateId;
            ScheduleIds = scheduleIds;
        }
    }
}