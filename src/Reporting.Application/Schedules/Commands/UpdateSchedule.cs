using AHI.Infrastructure.Service.Tag.Model;
using MediatR;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Handler.Command;
using Scheduler.Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reporting.Application.Command
{
    public class UpdateSchedule : UpsertTagCommand, IRequest<ScheduleDto>, IUpsertSchedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? CronExpressionId { get; set; }
        public string CronDescription { get; set; }
        public bool? IsSwitchedToCron { get; set; }
        public string Cron { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }
        public IEnumerable<int> Templates { get; set; }
        public ICollection<UpdateScheduleContact> Contacts { get; set; } = new List<UpdateScheduleContact>();
        public IEnumerable<int> Jobs { get; set; }

        private static Func<UpdateSchedule, string, Domain.Entity.Schedule> Converter = Projection.Compile();

        public static Expression<Func<UpdateSchedule, string, Domain.Entity.Schedule>> Projection
        {
            get
            {
                return (command, endpoint) => new Domain.Entity.Schedule(command.Contacts.Select((contact, index) => UpdateScheduleContact.Create(command.Id, contact, index)))
                {
                    Id = command.Id,
                    Name = command.Name,
                    Type = command.Type,
                    CronExpressionId = command.CronExpressionId,
                    IsSwitchedToCron = command.IsSwitchedToCron ?? false,
                    Cron = command.Cron,
                    CronDescription = CronJobHelper.GenerateDescription(command.Cron),
                    TimeZoneName = command.TimeZoneName,
                    Endpoint = $"{endpoint}/{Endpoint.ENDPOINT_BY_TYPE[command.Type]}",
                    Method = HttpClientMethod.POST,
                    Start = command.Start,
                    End = command.End,
                    ScheduleTemplates = command.Type == ScheduleType.REPORT_AND_SEND ? command.Templates.Distinct().Select(templateId => new Domain.Entity.ScheduleTemplate { TemplateId = templateId }).ToList() : new List<Domain.Entity.ScheduleTemplate>(),
                    Period = command.Period,
                    ScheduleJobs = command.Type == ScheduleType.SEND ? command.Jobs.Distinct().Select(jobId => new Domain.Entity.ScheduleJob { JobId = jobId }).ToList() : new List<Domain.Entity.ScheduleJob>()
                };
            }
        }

        public static Domain.Entity.Schedule Create(UpdateSchedule command, string endpoint)
        {
            if (command == null)
                return null;
            return Converter(command, endpoint);
        }

        public void AssignDefaultValueFromDb(Domain.Entity.Schedule scheduleEntity, IEnumerable<Operation> operations)
        {
            Name = !HasUpdate(nameof(Name), operations) ? scheduleEntity.Name : Name;
            Type = !HasUpdate(nameof(Type), operations) ? scheduleEntity.Type : Type;
            Cron = !HasUpdate(nameof(Cron), operations) ? scheduleEntity.Cron : Cron;
            CronExpressionId = !HasUpdate(nameof(CronExpressionId), operations) ? scheduleEntity.CronExpressionId : CronExpressionId;
            CronDescription = !HasUpdate(nameof(CronDescription), operations) ? scheduleEntity.CronDescription : CronDescription;
            IsSwitchedToCron = !HasUpdate(nameof(IsSwitchedToCron), operations) ? scheduleEntity.IsSwitchedToCron : IsSwitchedToCron;
            TimeZoneName = !HasUpdate(nameof(TimeZoneName), operations) ? scheduleEntity.TimeZoneName : TimeZoneName;
            Start = !HasUpdate(nameof(Start), operations) ? scheduleEntity.Start : Start;
            End = !HasUpdate(nameof(End), operations) ? scheduleEntity.End : End;
            Templates = !HasUpdate(nameof(Templates), operations) ? scheduleEntity.ScheduleTemplates.Select(x => x.TemplateId) : Templates;
            Jobs = !HasUpdate(nameof(Jobs), operations) ? scheduleEntity.ScheduleJobs.Select(x => x.JobId) : Jobs;
            Period = !HasUpdate(nameof(Period), operations) ? scheduleEntity.Period : Period;
        }

        private bool HasUpdate(string propertyName, IEnumerable<Operation> operations)
        {
            return operations.Any(patch => string.Equals(patch.path, string.Concat("/", propertyName), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}