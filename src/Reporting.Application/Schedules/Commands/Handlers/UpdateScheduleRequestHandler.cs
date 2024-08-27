using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class UpdateScheduleRequestHandler : IRequestHandler<UpdateSchedule, ScheduleDto>
    {
        private readonly IScheduleService _service;

        public UpdateScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<ScheduleDto> Handle(UpdateSchedule request, CancellationToken cancellationToken)
        {
            return _service.UpdateScheduleAsync(request);
        }
    }
}