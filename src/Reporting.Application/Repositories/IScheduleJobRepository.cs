using AHI.Infrastructure.Repository.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Repository
{
    public interface IScheduleJobRepository : IRepository<Domain.Entity.ScheduleJob, int>
    {
        Task DeleteByScheduleIdAsync(int scheduleId);
    }
}