using System.Threading.Tasks;
using AHI.Infrastructure.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Schedule.Command;
using System.Threading;
using System.Collections.Generic;

namespace Reporting.Application.Service.Abstraction
{
    public interface IScheduleService : ISearchService<Domain.Entity.Schedule, int, SearchSchedule, ScheduleDto>
    {
        Task<ScheduleDto> GetScheduleByIdAsync(GetScheduleById command);
        Task<ScheduleDto> AddScheduleAsync(AddSchedule command);
        Task<ScheduleDto> UpdateScheduleAsync(UpdateSchedule command);
        Task<ScheduleDto> PartialUpdateScheduleAsync(PartialUpdateSchedule command);
        Task<BaseResponse> DeleteScheduleAsync(DeleteSchedule command);
        Task<IEnumerable<ScheduleDto>> ArchiveAsync(ArchiveSchedule command, CancellationToken cancellationToken);
        Task<bool> VerifyArchiveAsync(VerifySchedule command, CancellationToken cancellationToken);
        Task<bool> RetrieveAsync(RetrieveSchedule command, CancellationToken cancellationToken);
    }
}