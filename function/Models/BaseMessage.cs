using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace Reporting.Function.Model
{
    public class BaseMessage : BusEvent
    {
        public string TenantId { get; set; }
        public string ProjectId { get; set; }
        public string SubscriptionId { get; set; }

        // not used
        public override string TopicName => "";
    }
}
