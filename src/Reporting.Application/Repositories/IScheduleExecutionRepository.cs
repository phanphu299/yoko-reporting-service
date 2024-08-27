using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Repository
{
    public interface IScheduleExecutionRepository : IRepository<ScheduleExecution, Guid>
    {
        Task UpdateStateAsync(Guid key, ExecutionState state, string executionParameter = null);
    }
}