namespace Reporting.Application.Notification
{
    public class WhatsAppNotifyMessage : BaseNotifyMessage
    {
        public string Content { get; set; }
        public string TemplateId { get; set; }

        public WhatsAppNotifyMessage(string content, string templateId, object payload)
            : base(payload)
        {
            Content = content;
            TemplateId = templateId;
        }
    }
}
