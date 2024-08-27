using MediatR;
using Reporting.Application.Models;
using Reporting.Application.Service.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class ExportReportByScheduleRequestHandler : IRequestHandler<ExportReportBySchedule, ActivityResponse>
    {
        private readonly IReportService _service;

        public ExportReportByScheduleRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<ActivityResponse> Handle(ExportReportBySchedule request, CancellationToken cancellationToken)
        {
            return _service.DownLoadReportsByScheduleAsync(request);
        }
    }
}