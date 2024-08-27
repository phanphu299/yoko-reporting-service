using System;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;
using Reporting.Persistence.Context;

namespace Reporting.Persistence.Repository
{
    public class ScheduleExecutionRepository : GenericRepository<ScheduleExecution, Guid>, IScheduleExecutionRepository
    {
        public readonly ReportingDbContext _context;

        public ScheduleExecutionRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<ScheduleExecution> AsQueryable()
        {
            return _context.ScheduleExecutions.Include(x => x.Schedule).AsQueryable();
        }

        public async Task UpdateStateAsync(Guid key, ExecutionState state, string executionParameter = null)
        {
            var trackingEntity = await _context.ScheduleExecutions.FirstOrDefaultAsync(x => x.Id == key);
            if (trackingEntity != null)
            {
                trackingEntity.State = state;
                if (!string.IsNullOrEmpty(executionParameter))
                    trackingEntity.ExecutionParam = executionParameter;
            }
        }

        protected override void Update(ScheduleExecution requestObject, ScheduleExecution targetObject)
        {
            targetObject.State = requestObject.State;
            targetObject.RetryCount = requestObject.RetryCount;
            targetObject.ExecutionParam = requestObject.ExecutionParam;

            targetObject.RetryJobId ??= requestObject.RetryJobId;

            targetObject.UpdatedUtc = DateTime.UtcNow;
        }
    }
}