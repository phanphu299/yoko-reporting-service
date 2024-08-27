namespace Reporting.Application.Notification
{
    public class BaseNotifyMessage
    {
        public object Payload { get; set; }

        public BaseNotifyMessage(object payload)
        {
            Payload = payload;
        }
    }
}