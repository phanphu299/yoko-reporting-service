using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Extension;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Reporting.Application.Command
{
    public class GenerateReport : IRequest<BaseResponse>
    {
        public GenerateReport()
        {
        }

        public GenerateReport(Domain.Entity.FailedSchedule failedSchedule, IEnumerable<int> templates)
        {
            JobId = failedSchedule.JobId;
            TimeZoneName = failedSchedule.TimeZoneName;
            ExecutionTime = failedSchedule.ExecutionTime;
            NextExecutionTime = failedSchedule.NextExecutionTime;
            PreviousExecutionTime = failedSchedule.PreviousExecutionTime;
            Templates = templates;
        }

        public string JobId { get; set; }
        public string TimeZoneName { get; set; }
        public long ExecutionTime { get; set; }
        public long NextExecutionTime { get; set; }
        public long PreviousExecutionTime { get; set; }
        public string DateTimeFormat { get; set; }
        public IEnumerable<int> Templates { get; set; } = new List<int>();
        public int? TemplateId { get; set; } // for old schedule

        public class GenerateTime
        {
            public DateTime FromDateUtc { get; set; }
            public DateTime ToDateUtc { get; set; }
            public DateTime ExecutionTimeUtc { get; set; }
            public DateTime FromDateLocal { get; set; }
            public DateTime ToDateLocal { get; set; }
            public DateTime ExecutionTimeLocal { get; set; }
            public string FromDateLocalInFormat { get; set; }
            public string ToDateLocalInFormat { get; set; }
            public long FromDateUtcInMilliseconds { get; set; }
            public long ToDateInUtcMilliseconds { get; set; }
        }

        public class GenerateFileInfo
        {
            public int StorageId { get; set; }
            public string Name { get; set; }
            public string OutputTypeId { get; set; }
            public string StorageUrl { get; set; }
            public string FileName { get; set; }
        }

        public GenerateTime ExtractFromDateToDate(string timeZoneName, string dateTimeFormat)
        {
            var fromDateUtc = StringExtensions.UnixTimeStampToDateTime(PreviousExecutionTime.ToString());
            var toDateUtc = StringExtensions.UnixTimeStampToDateTime(ExecutionTime.ToString());

            var executionTimeUtc = StringExtensions.UnixTimeStampToDateTime(ExecutionTime.ToString());

            var fromDateLocal = fromDateUtc.ToLocalDateTime(timeZoneName);
            var toDateLocal = toDateUtc.ToLocalDateTime(timeZoneName);
            var executionTimeLocal = executionTimeUtc.ToLocalDateTime(timeZoneName);

            var fromDateLocalInFormat = fromDateLocal.ToString(dateTimeFormat);
            var toDateLocalInFormat = toDateLocal.ToString(dateTimeFormat);

            var fromDateUtcInMilliseconds = new DateTimeOffset(fromDateUtc).ToUnixTimeMilliseconds();
            var toDateInUtcMilliseconds = new DateTimeOffset(toDateUtc).ToUnixTimeMilliseconds();

            return new GenerateTime
            {
                FromDateUtc = fromDateUtc,
                ToDateUtc = toDateUtc,
                ExecutionTimeUtc = executionTimeUtc,
                FromDateLocal = fromDateLocal,
                ToDateLocal = toDateLocal,
                ExecutionTimeLocal = executionTimeLocal,
                FromDateLocalInFormat = fromDateLocalInFormat,
                ToDateLocalInFormat = toDateLocalInFormat,
                FromDateUtcInMilliseconds = fromDateUtcInMilliseconds,
                ToDateInUtcMilliseconds = toDateInUtcMilliseconds
            };
        }

        public static Expression<Func<GenerateFileInfo, GenerateTime, Domain.Entity.Report>> Projection
        {
            get
            {
                return (generatedFile, generateTime) => new Domain.Entity.Report
                {
                    Name = generatedFile.Name,
                    StorageId = generatedFile.StorageId,
                    OutputTypeId = generatedFile.OutputTypeId,
                    StorageUrl = generatedFile.StorageUrl,
                    FileName = generatedFile.FileName,
                    FromDateUtc = generateTime.FromDateUtc,
                    ToDateUtc = generateTime.ToDateUtc,
                };
            }
        }

        public static Domain.Entity.Report Create(GenerateFileInfo generatedFile, GenerateTime generateTime)
        {
            return Projection.Compile().Invoke(generatedFile, generateTime);
        }
    }
}