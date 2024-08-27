using System;
using System.Linq.Expressions;
using Reporting.Application.Constant;
using Reporting.Application.Command.Model;
using MediatR;
using Scheduler.Application.Helper;
using System.Collections.Generic;
using System.Linq;
using Reporting.Application.Handler.Command;
using AHI.Infrastructure.Service.Tag.Model;

namespace Reporting.Application.Command
{
    public class AddSchedule : UpsertTagCommand, IRequest<ScheduleDto>, IUpsertSchedule
    {
        public string Name { get; set; }
        public Guid? CronExpressionId { get; set; }
        public bool IsSwitchedToCron { get; set; }
        public string Cron { get; set; }
        public string CronDescription { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }
        public IEnumerable<int> Templates { get; set; }
        public ICollection<AddScheduleContact> Contacts { get; set; } = new List<AddScheduleContact>();
        public IEnumerable<int> Jobs { get; set; }

        private static Func<AddSchedule, string, Domain.Entity.Schedule> Converter = Projection.Compile();

        public static Expression<Func<AddSchedule, string, Domain.Entity.Schedule>> Projection
        {
            get
            {
                return (command, endpoint) => new Domain.Entity.Schedule(command.Contacts.Select((contact, index) => AddScheduleContact.Create(contact, index)))
                {
                    Name = command.Name,
                    CronExpressionId = command.CronExpressionId,
                    IsSwitchedToCron = command.IsSwitchedToCron,
                    Cron = command.Cron,
                    CronDescription = CronJobHelper.GenerateDescription(command.Cron),
                    TimeZoneName = command.TimeZoneName,
                    Endpoint = $"{endpoint}/{Endpoint.ENDPOINT_BY_TYPE[command.Type]}",
                    Method = HttpClientMethod.POST,
                    Start = command.Start,
                    End = command.End,
                    JobId = Guid.NewGuid().ToString(),
                    Type = command.Type,
                    ScheduleTemplates = command.Type == ScheduleType.REPORT_AND_SEND ? command.Templates.Distinct().Select(templateId => new Domain.Entity.ScheduleTemplate { TemplateId = templateId }).ToList() : new List<Domain.Entity.ScheduleTemplate>(),
                    Period = command.Period,
                    ScheduleJobs = command.Type == ScheduleType.SEND ? command.Jobs.Distinct().Select(jobId => new Domain.Entity.ScheduleJob { JobId = jobId }).ToList() : new List<Domain.Entity.ScheduleJob>()
                };
            }
        }

        public static Domain.Entity.Schedule Create(AddSchedule command, string endpoint)
        {
            if (command == null)
                return null;
            return Converter(command, endpoint);
        }
    }
}