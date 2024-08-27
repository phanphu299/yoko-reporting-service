using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class AddScheduleRequestHandler : IRequestHandler<AddSchedule, ScheduleDto>
    {
        private readonly IScheduleService _service;

        public AddScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }

        public Task<ScheduleDto> Handle(AddSchedule request, CancellationToken cancellationToken)
        {
            return _service.AddScheduleAsync(request);
        }
    }
}