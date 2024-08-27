using System;
using System.Collections.Generic;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class BuildReportFile
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public SimpleTemplateByIdDto Template { get; set; }
        public IEnumerable<DataTableResult> Data { get; set; }

        public BuildReportFile()
        {
        }

        public BuildReportFile(DateTime fromDate, DateTime toDate, SimpleTemplateByIdDto template, IEnumerable<DataTableResult> data)
        {
            FromDate = fromDate;
            ToDate = toDate;
            Template = template;
            Data = data;
        }
    }
}