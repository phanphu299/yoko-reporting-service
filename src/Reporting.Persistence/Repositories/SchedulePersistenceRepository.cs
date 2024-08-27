using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;
using System.Collections.Generic;
using Reporting.Domain.Entity;
using Reporting.Application.Constant;

namespace Reporting.Persistence.Repository
{
    public class SchedulePersistenceRepository : GenericRepository<Schedule, int>, IScheduleRepository
    {
        private readonly ReportingDbContext _context;

        public SchedulePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Schedule> AsQueryable()
        {
            return _context.Schedules
                        .Include(s => s.ScheduleTemplates)
                        .Include(s => s.ScheduleJobs)
                        .Include(x => x.EntityTags)
                        .Where(x => !x.EntityTags.Any() || x.EntityTags.Any(a => a.EntityType == EntityTypeConstants.REPORT_SCHEDULE));
        }

        protected override void Update(Schedule requestObject, Schedule targetObject)
        {
            targetObject.Name = requestObject.Name;
            targetObject.Type = requestObject.Type;
            targetObject.CronDescription = requestObject.CronDescription;
            targetObject.CronExpressionId = requestObject.CronExpressionId;
            targetObject.IsSwitchedToCron = requestObject.IsSwitchedToCron;
            targetObject.Cron = requestObject.Cron;
            targetObject.TimeZoneName = requestObject.TimeZoneName;
            targetObject.Endpoint = requestObject.Endpoint;
            targetObject.Method = requestObject.Method;
            targetObject.AdditionalParams = requestObject.AdditionalParams;
            targetObject.Start = requestObject.Start;
            targetObject.End = requestObject.End;

            foreach (var targetContact in targetObject.Contacts)
            {
                var requestContact = requestObject.Contacts.FirstOrDefault(c => c.Id == targetContact.Id);
                if (requestContact != null)
                {
                    targetContact.SequentialNumber = requestContact.SequentialNumber;
                }
            }
            _context.RemoveRange(targetObject.Contacts.Except(requestObject.Contacts, Extension.EntityComparer.ContactIdComparer));
            _context.AddRange(requestObject.Contacts.Except(targetObject.Contacts, Extension.EntityComparer.ContactIdComparer));

            targetObject.UpdatedUtc = DateTime.UtcNow;
            targetObject.ScheduleTemplates = requestObject.ScheduleTemplates;
            targetObject.ScheduleJobs = requestObject.ScheduleJobs;
            targetObject.Period = requestObject.Period;
        }

        public override async Task<Schedule> UpdateAsync(int id, Schedule e)
        {
            var trackingEntity = await FindAsync(id);
            if (trackingEntity != null)
            {
                Update(e, trackingEntity);
            }
            return trackingEntity;
        }

        public async Task UpdateLastRunAsync(int id, DateTime? lastRunUtc)
        {
            if (!lastRunUtc.HasValue)
                return;

            var trackingEntity = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);
            if (trackingEntity != null)
                trackingEntity.LastRunUtc = lastRunUtc;
        }

        public override Task<Schedule> FindAsync(int id)
        {
            return _context.Schedules.Include(x => x.ScheduleTemplates)
                                    .Include(x => x.Contacts)
                                    .Include(x => x.ScheduleJobs)
                                    .Include(x => x.EntityTags)
                                    .Where(x => x.Id == id && (!x.EntityTags.Any() || x.EntityTags.Any(a => a.EntityType == EntityTypeConstants.REPORT_SCHEDULE)))
                                    .FirstOrDefaultAsync();
        }

        public async Task RetrieveAsync(IEnumerable<Schedule> schedules)
        {
            _context.Database.SetCommandTimeout(RetrieveConstants.TIME_OUT);
            // enable insert identity
            var disableSql = "SET IDENTITY_INSERT schedules ON;";
            await _context.Database.ExecuteSqlRawAsync(disableSql);

            await _context.Schedules.AddRangeAsync(schedules);
            // Need save change before disable identity
            await _context.SaveChangesAsync();

            // disable insert identity
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT schedules OFF;");
        }
    }
}