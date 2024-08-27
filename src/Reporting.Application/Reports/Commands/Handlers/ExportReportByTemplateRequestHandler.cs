using MediatR;
using Reporting.Application.Models;
using Reporting.Application.Service.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Reporting.Application.Command.Handler
{
    public class ExportReportByTemplateRequestHandler : IRequestHandler<ExportReportByTemplate, ActivityResponse>
    {
        private readonly IReportService _service;

        public ExportReportByTemplateRequestHandler(IReportService service)
        {
            _service = service;
        }

        public Task<ActivityResponse> Handle(ExportReportByTemplate request, CancellationToken cancellationToken)
        {
            return _service.DownLoadReportsByTemplateAsync(request);
        }
    }
}