using System.Collections.Generic;
using Newtonsoft.Json;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Command.Model
{
    public interface IExecutionParameter
    {
        [JsonIgnore]
        public long ExecutionTimestamp { get; }
        [JsonIgnore]
        public int TotalGenerated { get; }
        [JsonIgnore]
        public int TargetCount { get; }
        [JsonIgnore]
        public ExecutionState State { get; }

        public void AddRange(IEnumerable<GenerateReportDto> newReports);
        
        public GenerateReport BuildRequest();
        public FailedSchedule CreateFailedSchedule();
    }
}