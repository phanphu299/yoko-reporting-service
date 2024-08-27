using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Model;
using Reporting.Application.Constant;

namespace Reporting.Application.Command.Model
{
    public class ScheduleDto : TagDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? CronExpressionId { get; set; }
        public string CronDescription { get; set; }
        public bool IsSwitchedToCron { get; set; }
        public string Cron { get; set; }
        public string TimeZoneName { get; set; }
        public string Endpoint { get; set; }
        public string AdditionalParams { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string JobId { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime? LastRunUtc { get; set; }
        public IEnumerable<int> Templates { get; set; }
        public IEnumerable<ScheduleContactDto> Contacts { get; set; } = new List<ScheduleContactDto>();
        public IEnumerable<string> WarningCodes { get; set; }
        public IEnumerable<int> Jobs { get; set; }

        private static Func<Domain.Entity.Schedule, IEnumerable<string>, ScheduleDto> Converter = Projection.Compile();
        private static Func<Domain.Entity.Schedule, ScheduleDto> DtoConverter = DtoProjection.Compile();
        private static Func<ScheduleDto, Domain.Entity.Schedule> EntityConverter = EntityProjection.Compile();

        public static Expression<Func<Domain.Entity.Schedule, IEnumerable<string>, ScheduleDto>> Projection
        {
            get
            {
                return (entity, warnings) => new ScheduleDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CronDescription = entity.CronDescription,
                    Templates = entity.ScheduleTemplates.Select(x => x.TemplateId),
                    CronExpressionId = entity.CronExpressionId,
                    IsSwitchedToCron = entity.IsSwitchedToCron,
                    Cron = entity.Cron,
                    TimeZoneName = entity.TimeZoneName,
                    Endpoint = entity.Endpoint,
                    AdditionalParams = entity.AdditionalParams,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    Start = entity.Start,
                    End = entity.End,
                    JobId = entity.JobId,
                    LastRunUtc = entity.LastRunUtc,
                    Type = entity.Type,
                    Contacts = entity.Contacts != null
                                ? entity.Contacts
                                        .Select(contact => ScheduleContactDto.CreateDto(contact))
                                        .OrderBy(x => x.SequentialNumber)
                                : null,
                    WarningCodes = warnings,
                    Period = entity.Period,
                    Jobs = entity.ScheduleJobs.Select(x => x.JobId),
                    Tags = entity.EntityTags.MappingTagDto()
                };
            }
        }

        public static Expression<Func<Domain.Entity.Schedule, ScheduleDto>> DtoProjection
        {
            get
            {
                return entity => new ScheduleDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CronDescription = entity.CronDescription,
                    Templates = entity.ScheduleTemplates.Select(x => x.TemplateId),
                    CronExpressionId = entity.CronExpressionId,
                    IsSwitchedToCron = entity.IsSwitchedToCron,
                    Cron = entity.Cron,
                    TimeZoneName = entity.TimeZoneName,
                    Endpoint = entity.Endpoint,
                    AdditionalParams = entity.AdditionalParams,
                    ResourcePath = entity.ResourcePath,
                    Start = entity.Start,
                    End = entity.End,
                    JobId = entity.JobId,
                    Method = entity.Method,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    Type = entity.Type,
                    Contacts = entity.Contacts.Select(contact => ScheduleContactDto.CreateDto(contact)),
                    Period = entity.Period,
                    Jobs = entity.ScheduleJobs.Select(x => x.JobId)
                };
            }
        }

        public static Expression<Func<ScheduleDto, Domain.Entity.Schedule>> EntityProjection
        {
            get
            {
                return entity => new Domain.Entity.Schedule(entity.Contacts.Select(contact => ScheduleContactDto.CreateEntity(contact)))
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CronDescription = entity.CronDescription,
                    CronExpressionId = entity.CronExpressionId,
                    IsSwitchedToCron = entity.IsSwitchedToCron,
                    Cron = entity.Cron,
                    TimeZoneName = entity.TimeZoneName,
                    Endpoint = entity.Endpoint,
                    AdditionalParams = entity.AdditionalParams,
                    ResourcePath = entity.ResourcePath,
                    Start = entity.Start,
                    End = entity.End,
                    JobId = entity.JobId,
                    Method = entity.Method,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow,
                    Type = entity.Type,
                    ScheduleTemplates = entity.Type == ScheduleType.REPORT_AND_SEND ? entity.Templates.Select(templateId => new Domain.Entity.ScheduleTemplate { TemplateId = templateId }).ToList() : new List<Domain.Entity.ScheduleTemplate>(),
                    Period = entity.Period,
                    ScheduleJobs = entity.Type == ScheduleType.SEND ? entity.Jobs.Select(jobId => new Domain.Entity.ScheduleJob { JobId = jobId }).ToList() : new List<Domain.Entity.ScheduleJob>()
                };
            }
        }

        public static ScheduleDto CreateArchive(Domain.Entity.Schedule entity)
        {
            if (entity == null)
                return null;
            return DtoConverter(entity);
        }

        public static Domain.Entity.Schedule Create(ScheduleDto entity)
        {
            if (entity == null)
                return null;
            return EntityConverter(entity);
        }

        public static ScheduleDto Create(Domain.Entity.Schedule entity)
        {
            if (entity == null)
                return null;
            return Converter(entity, null);
        }

        public static ScheduleDto Create(Domain.Entity.Schedule entity, IEnumerable<string> warningCodes)
        {
            if (entity == null)
                return null;
            return Converter(entity, warningCodes);
        }
    }
}