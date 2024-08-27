using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Schedule.Command.Handler
{
    public class RetrieveScheduleRequestHandler : IRequestHandler<RetrieveSchedule, BaseResponse>
    {
        private readonly IScheduleService _service;

        public RetrieveScheduleRequestHandler(IScheduleService service)
        {
            _service = service;
        }
        
        public async Task<BaseResponse> Handle(RetrieveSchedule request, CancellationToken cancellationToken)
        {
            var result = await _service.RetrieveAsync(request, cancellationToken);
            return new BaseResponse(result, null);
        }
    }
}
