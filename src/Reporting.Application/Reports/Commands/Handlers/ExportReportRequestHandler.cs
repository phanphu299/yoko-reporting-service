using MediatR;
using Reporting.Application.Models;
using Reporting.Application.Service.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class ExportReportRequestHandler : IRequestHandler<ExportReport, ActivityResponse>
    {
        private readonly IReportService _service;

        public ExportReportRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<ActivityResponse> Handle(ExportReport request, CancellationToken cancellationToken)
        {
            return _service.DownloadReportsAsync(request);
        }
    }
}