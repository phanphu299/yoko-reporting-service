using Reporting.Application.Constant;
using Reporting.Application.Extension;
using System;

namespace Reporting.Application.Command
{
    public class PreviewReportBase
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string OutputTypeId { get; set; }
        public int TemplateId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromDateLocalInFormat { get; set; }
        public string ToDateLocalInFormat { get; set; }
        public long FromDateInMilliseconds { get; set; }
        public long ToDateInMilliseconds { get; set; }

        public void ExtractFromDateToDate(string timeZoneName, string dateTimeFormat)
        {
            FromDateLocalInFormat = FromDate.ToLocalDateTime(timeZoneName).ToString(dateTimeFormat);
            ToDateLocalInFormat = ToDate.ToLocalDateTime(timeZoneName).ToString(dateTimeFormat);
            FromDateInMilliseconds = new DateTimeOffset(DateTime.SpecifyKind(FromDate, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
            ToDateInMilliseconds = new DateTimeOffset(DateTime.SpecifyKind(ToDate, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
        }

        public void SetDefaultOutput()
        {
            if (string.IsNullOrEmpty(OutputTypeId))
            {
                OutputTypeId = OutputType.PDF;
            }
        }
    }
}