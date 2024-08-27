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
    public class ScheduleJobPersistenceRepository : GenericRepository<ScheduleJob, int>, IScheduleJobRepository
    {
        private readonly ReportingDbContext _context;

        public ScheduleJobPersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task DeleteByScheduleIdAsync(int scheduleId)
        {
            var scheduleJobs = await _context.ScheduleJobs
                                                        .AsQueryable()
                                                        .Where(x => x.ScheduleId == scheduleId ||
                                                                    x.JobId == scheduleId)
                                                        .ToListAsync();

            _context.ScheduleJobs.RemoveRange(scheduleJobs);
        }

        protected override void Update(ScheduleJob requestObject, ScheduleJob targetObject)
        {
            throw new NotImplementedException();
        }
    }
}