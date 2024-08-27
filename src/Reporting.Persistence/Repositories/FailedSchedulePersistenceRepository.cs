using AHI.Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Domain.Entity;
using Reporting.Persistence.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Persistence.Repository
{
    public class FailedSchedulePersistenceRepository : GenericRepository<Domain.Entity.FailedSchedule, int>, IFailedScheduleRepository
    {
        private readonly ReportingDbContext _context;

        public FailedSchedulePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.FailedSchedule> AsQueryable() => _context.FailedSchedules;

        public async Task DeleteByScheduleIdAsync(int scheduleId)
        {
            var failedSchedules = await AsQueryable().AsNoTracking().Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.FailedSchedules.RemoveRange(failedSchedules);
        }

        protected override void Update(FailedSchedule requestObject, FailedSchedule targetObject)
        {
            targetObject.ScheduleName = requestObject.ScheduleName;
            targetObject.ScheduleId = requestObject.ScheduleId;
            targetObject.JobId = requestObject.JobId;
            targetObject.TimeZoneName = requestObject.TimeZoneName;
            targetObject.ExecutionTime = requestObject.ExecutionTime;
            targetObject.NextExecutionTime = requestObject.NextExecutionTime;
            targetObject.PreviousExecutionTime = requestObject.PreviousExecutionTime;
            targetObject.UpdatedUtc = DateTime.UtcNow;
        }
    }
}