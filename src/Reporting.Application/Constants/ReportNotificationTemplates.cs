namespace Reporting.Application.Constant
{
    public static class ReportNotificationTemplates
    {
        public const string MainContent = "Hi {{toEmail}},<br>{{scheduleName}} was {{state}}";

        public const string ReportAndSendScheduleFailedList = " ({{failedCount}} failed report(s))<br>{{failedList}}";

        public const string SendScheduleIssueCount = " ({{issueCount}} issue(s))";
        public const string SendScheduleFailedList = "<br>+ {{failedCount}} failed:<br>{{failedList}}";
        public const string SendSchedulePartialSuccessList = "<br>+ {{partialSuccessCount}} partial success:<br>{{partialSuccessList}}";

        public const string DownloadLinkContent = "<br>Please get the report file at: {{url}}";

        public const string EmailSubject = "OpreX Asset Health Insights - [Report]: {{scheduleName}}_{{timestamp}}";
        public const string EmailSignOff = "<br><br>Best Regards,<br>OpreX Asset Health Insight team<br>{{emailSupport}}" +
            "<span style=\"font-size: 14px; line-height: 140%;\"><a rel=\"noopener\" href=\"mailto:support@ahi.apps.yokogawa.com\" target=\"_blank\">support@ahi.apps.yokogawa.com</a></span>";
    }
}
