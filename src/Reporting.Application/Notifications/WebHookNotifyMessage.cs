namespace Reporting.Application.Notification
{
    public class WebHookNotifyMessage : BaseNotifyMessage
    {
        public string HtmlContent { get; set; }

        public WebHookNotifyMessage(string htmlContent, object payload)
            : base(payload)
        {
            HtmlContent = htmlContent;
        }
    }
}
