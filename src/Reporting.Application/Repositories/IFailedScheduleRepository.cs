using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IFailedScheduleRepository : IRepository<Domain.Entity.FailedSchedule, int>
    {
        Task DeleteByScheduleIdAsync(int scheduleId);
    }
}