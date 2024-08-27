namespace Reporting.Application.Notification
{
    public class EmailNotifyMessage : BaseNotifyMessage
    {
        public string TypeCode { get; set; } = "DEFAULT";
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        /// <summary>
        /// With `true` value, the notification-service will auto append the Header & Footer to the Html.
        /// </summary>
        public bool IncludeHeaderFooter { get; set; }

        public EmailNotifyMessage(string htmlContent, string subject, bool includeHeaderFooter, object payload)
            : base(payload)
        {
            Subject = subject;
            HtmlBody = htmlContent;
            IncludeHeaderFooter = includeHeaderFooter;
        }
    }
}