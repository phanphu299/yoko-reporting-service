using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class PartialUpdateScheduleRequestHandler : IRequestHandler<PartialUpdateSchedule, ScheduleDto>
    {
        private readonly IScheduleService _service;

        public PartialUpdateScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<ScheduleDto> Handle(PartialUpdateSchedule request, CancellationToken cancellationToken)
        {
            return _service.PartialUpdateScheduleAsync(request);
        }
    }
}