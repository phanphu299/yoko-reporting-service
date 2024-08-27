using MediatR;
using Reporting.Application.Models;
using Reporting.Application.Service.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class ExportPreviewReportRequestHandler : IRequestHandler<ExportPreviewReport, ActivityResponse>
    {
        private readonly IReportService _service;

        public ExportPreviewReportRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<ActivityResponse> Handle(ExportPreviewReport request, CancellationToken cancellationToken)
        {
            return _service.DownloadPreviewReportsAsync(request);
        }
    }
}