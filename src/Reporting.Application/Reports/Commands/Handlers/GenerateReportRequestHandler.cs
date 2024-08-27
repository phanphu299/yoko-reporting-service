using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using MediatR;
using AHI.Infrastructure.SharedKernel.Model;

namespace Reporting.Application.Command.Handler
{
    public class GenerateReportRequestHandler : IRequestHandler<GenerateReport, BaseResponse>
    {
        private readonly IReportService _service;

        public GenerateReportRequestHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<BaseResponse> Handle(GenerateReport request, CancellationToken cancellationToken)
        {
            await _service.InitGenerateReportAsync(request);
            return BaseResponse.Success;
        }
    }
}