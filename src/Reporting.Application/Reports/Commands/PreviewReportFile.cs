using AHI.Infrastructure.MultiTenancy.Internal;
using Reporting.Application.Command.Model;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Command
{
    public class PreviewReportFile : BuildReportFile
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string Upn { get; set; }
        public string TimeZoneName { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }

        public PreviewReportFile()
        {
        }

        public PreviewReportFile(Guid activityId, string upn, string timeZoneName, TenantContext tenantContext, DateTime fromDate, DateTime toDate, SimpleTemplateByIdDto template, IEnumerable<DataTableResult> data)
            : base(fromDate, toDate, template, data)
        {
            ActivityId = activityId;
            Upn = upn;
            TimeZoneName = timeZoneName;
            TenantId = tenantContext.TenantId;
            SubscriptionId = tenantContext.SubscriptionId;
            ProjectId = tenantContext.ProjectId;
        }

        public string GetPreviewkey() => $"preview_{ProjectId}_template_{Template.Id}_{TimeZoneName}_{FromDate.ToString("yyyy-MM-dd-HH-mm")}_{ToDate.ToString("yyyy-MM-dd-HH-mm")}".ToLower();
    }
}