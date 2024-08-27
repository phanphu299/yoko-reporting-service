using System.Collections.Generic;

namespace Reporting.Application.Command.Model
{
    public class WhatsAppTemplateOptions
    {
        public WhatsAppTemplateDefinition ReportSchedulesSuccess { get; set; }
        public WhatsAppTemplateDefinition ReportSchedulesFail { get; set; }
        public WhatsAppTemplateDefinition ReportAndSendSchedulePartial { get; set; }
        public WhatsAppTemplateDefinition SendSchedulePartialMixed { get; set; }
        public WhatsAppTemplateDefinition SendSchedulePartialFailed { get; set; }
        public WhatsAppTemplateDefinition SendSchedulePartial { get; set; }
    }

    public class WhatsAppTemplateDefinition
    {
        public string Id { get; set; }
        public List<string> Parameters { get; set; }
    }
}