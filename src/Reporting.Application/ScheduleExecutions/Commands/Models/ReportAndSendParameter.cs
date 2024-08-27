using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Command.Model
{
    public class ReportAndSendParameter : IExecutionParameter
    {
        [JsonProperty("request_data")]
        public ReportAndSendRequest Request { get; set; }
        public ICollection<ReportAndSendResult> Generated { get; set; }

        public long ExecutionTimestamp => Request.ExecutionTime;
        public int TotalGenerated => Generated.Count;
        public int TargetCount => Request.Templates.Count();
        public ExecutionState State => TotalGenerated == TargetCount ? ExecutionState.FIN
                                     : TotalGenerated == 0 ? ExecutionState.FAIL
                                     : ExecutionState.PFIN;

        public void AddRange(IEnumerable<GenerateReportDto> newReports)
        {
            foreach (var report in newReports)
            {
                Generated.Add(new ReportAndSendResult(report.TemplateId.Value, report.Id));
            }
        }

        public GenerateReport BuildRequest()
        {
            return new GenerateReport
            {
                JobId = Request.JobId,
                TimeZoneName = Request.TimeZoneName,
                ExecutionTime = Request.ExecutionTime,
                NextExecutionTime = Request.NextExecutionTime,
                PreviousExecutionTime = Request.PreviousExecutionTime,
                DateTimeFormat = Request.DateTimeFormat,
                Templates = Request.Templates.Except(Generated.Select(x => x.TemplateId))
            };
        }

        public FailedSchedule CreateFailedSchedule()
        {
            return new FailedSchedule
            {
                JobId = Request.JobId,
                TimeZoneName = Request.TimeZoneName,
                ExecutionTime = Request.ExecutionTime,
                NextExecutionTime = Request.NextExecutionTime,
                PreviousExecutionTime = Request.PreviousExecutionTime,
            };
        }

        public static ReportAndSendParameter Create(GenerateReport command)
        {
            return new ReportAndSendParameter()
            {
                Request = ReportAndSendRequest.Create(command),
                Generated = new List<ReportAndSendResult>()
            };
        }
    }

    public class ReportAndSendRequest
    {
        public string JobId { get; set; }
        public string TimeZoneName { get; set; }
        public long ExecutionTime { get; set; }
        public long NextExecutionTime { get; set; }
        public long PreviousExecutionTime { get; set; }
        public string DateTimeFormat { get; set; }
        public IEnumerable<int> Templates { get; set; }
        public static ReportAndSendRequest Create(GenerateReport command)
        {
            return new ReportAndSendRequest()
            {
                JobId = command.JobId,
                TimeZoneName = command.TimeZoneName,
                ExecutionTime = command.ExecutionTime,
                NextExecutionTime = command.NextExecutionTime,
                PreviousExecutionTime = command.PreviousExecutionTime,
                DateTimeFormat = command.DateTimeFormat,
                Templates = new List<int>(command.Templates),
            };
        }
    }

    public class ReportAndSendResult
    {
        public int TemplateId { get; set; }
        public int ReportId { get; set; }

        public ReportAndSendResult(int templateId, int reportId)
        {
            TemplateId = templateId;
            ReportId = reportId;
        }
    }
}