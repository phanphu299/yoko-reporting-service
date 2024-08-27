using System.Collections.Generic;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Notification;

namespace Reporting.Application.Event
{
    public class ScheduleTriggeredEvent : BusEvent
    {
        public override string TopicName => EventTopics.SCHEDULE_TRIGGERED;
        public string TenantId { get; set; }
        public string ProjectId { get; set; }
        public string SubscriptionId { get; set; }
        public IDictionary<string, BaseNotifyMessage> Payloads { get; set; }
        public IEnumerable<ScheduleContactSimpleDto> Contacts { get; set; }
        public string SourceType { get; set; }
        public string SourceId { get; set; }

        public ScheduleTriggeredEvent(IDictionary<string, BaseNotifyMessage> payloads, IEnumerable<ScheduleContactSimpleDto> contacts, ITenantContext tenantContext, string sourceId, string sourceType = nameof(EventTopics.SCHEDULE_TRIGGERED))
        {
            Contacts = contacts;
            Payloads = payloads;
            TenantId = tenantContext.TenantId;
            SubscriptionId = tenantContext.SubscriptionId;
            ProjectId = tenantContext.ProjectId;
            SourceId = sourceId;
            SourceType = sourceType;
        }
    }
}