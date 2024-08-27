using System.Collections.Generic;

namespace Reporting.Application.Constant
{
    public class Endpoint
    {
        public const string GENERATE_REPORT = "rpt/reports/generate";
        public const string RETRY_GENERATE_REPORT = "rpt/reports/executions/{{executionId}}/retry";
        public const string COLLECT_REPORT = "rpt/reports/collect";
        public const string RETRY_COLLECT_REPORT = "rpt/reports/collect/retry";
        public const string ENGLISH_TRANSLATION_FILE = "languages/asset-dashboard/en-us.json";
        public const string EXPORT_NOTIFICATION_END_POINT = "ntf/notifications/export/notify";
        public static IDictionary<string, string> ENDPOINT_BY_TYPE = new Dictionary<string, string> {
            { ScheduleType.REPORT_AND_SEND, GENERATE_REPORT },
            { ScheduleType.SEND, COLLECT_REPORT }
        };
    }
}