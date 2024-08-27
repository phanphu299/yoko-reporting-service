namespace Reporting.Application.Command.Model
{
    public class RetryOptions
    {
        public ReportAndSendOptions ReportAndSend { get; set; } = new ReportAndSendOptions();
        public SendOptions Send { get; set; } = new SendOptions();

        public class ReportAndSendOptions
        {
            public int RetryCount { get; set; } = 2;
            public int RetryDelay { get; set; } = 5;
        }

        public class SendOptions
        {
            public int RetryCount { get; set; } = 3;
            public int RetryDelay { get; set; } = 5;
        }
    }
}