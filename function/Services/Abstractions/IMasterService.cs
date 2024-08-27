using System.Collections.Generic;
using System.Threading.Tasks;
using Reporting.Function.Model;

namespace Reporting.Function.Service.Abstraction
{
    public interface IMasterService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectBySubscriptionAsync();
    }
}