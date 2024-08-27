using AHI.Infrastructure.Repository.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Repository
{
    public interface IScheduleRepository : IRepository<Domain.Entity.Schedule, int>
    {
        Task RetrieveAsync(IEnumerable<Domain.Entity.Schedule> schedules);
        Task UpdateLastRunAsync(int id, DateTime? lastRunUtc);
    }
}