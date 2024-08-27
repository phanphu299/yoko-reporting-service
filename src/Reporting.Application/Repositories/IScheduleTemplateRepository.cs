using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Repository
{
    public interface IScheduleTemplateRepository
    {
        Task DeleteByScheduleIdAsync(int scheduleId);
        Task<bool> IsBeingUsedAsync(IEnumerable<int> templateIds);
    }
}