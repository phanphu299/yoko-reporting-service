using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Schedule.Command.Handler
{
    public class ArchiveScheduleRequestHandler : IRequestHandler<ArchiveSchedule, IEnumerable<ScheduleDto>>
    {
        private readonly IScheduleService _service;
        
        public ArchiveScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<IEnumerable<ScheduleDto>> Handle(ArchiveSchedule request, CancellationToken cancellationToken)
        {
            return _service.ArchiveAsync(request, cancellationToken);
        }
    }
}
