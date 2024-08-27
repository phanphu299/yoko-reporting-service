using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Command.Handler
{
    public class TriggerExecutionRequestHandler : IRequestHandler<TriggerExecution>
    {
        private readonly IScheduleExecutionService _service;

        public TriggerExecutionRequestHandler(IScheduleExecutionService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TriggerExecution request, CancellationToken cancellationToken)
        {
            await _service.TriggerExecutionAsync(request.ScheduleExecutionId, request.RetryJobId);
            return Unit.Value;
        }
    }
}