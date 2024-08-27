using System;
using System.Collections.Generic;

namespace Reporting.Function.Model
{
    public class ExportFileMessage
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string ObjectType { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public IEnumerable<string> Ids { get; set; }
        public string TemplateId { get; set; }
        public IEnumerable<string> ScheduleIds { get; set; }
        public string RequestedBy { get; set; }
        public string DateTimeFormat { get; set; }
        public string DateTimeOffset { get; set; }
    }
}