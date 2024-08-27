using AHI.Infrastructure.Repository.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Repository
{
    public interface IScheduleContactRepository : IRepository<Domain.Entity.SchedulerContact, Guid>
    {
        Task<IEnumerable<Domain.Entity.SchedulerContact>> GetScheduleContactsByScheduleIdAsync(int scheduleId);
    }
}