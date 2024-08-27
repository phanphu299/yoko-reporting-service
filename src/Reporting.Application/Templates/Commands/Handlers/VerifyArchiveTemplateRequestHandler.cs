using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Template.Command.Handler
{
    public class VerifyArchiveTemplateRequestHandler : IRequestHandler<VerifyArchivedTemplate, BaseResponse>
    {
        private readonly ITemplateService _service;

        public VerifyArchiveTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }
        
        public async Task<BaseResponse> Handle(VerifyArchivedTemplate request, CancellationToken cancellationToken)
        {
            var result = await _service.VerifyArchiveAsync(request, cancellationToken);
            return new BaseResponse(result, null);
        }
    }
}
