using System.Threading.Tasks;
using Reporting.Application.Command.Model;
using AHI.Infrastructure.SharedKernel.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IJobService
    {
        Task<JobDto> AddJobAsync(JobDto model);
        Task<JobDto> UpdateJobAsync(JobDto model);
        Task<BaseResponse> DeleteJobAsync(string id);
    }
}