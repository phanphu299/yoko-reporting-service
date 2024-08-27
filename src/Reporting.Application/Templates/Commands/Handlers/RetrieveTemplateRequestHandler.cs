using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Template.Command.Handler
{
    public class RetrieveTemplateRequestHandler : IRequestHandler<RetrieveTemplate, BaseResponse>
    {
        private readonly ITemplateService _service;

        public RetrieveTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public async Task<BaseResponse> Handle(RetrieveTemplate request, CancellationToken cancellationToken)
        {
            var result = await _service.RetrieveAsync(request, cancellationToken);
            return new BaseResponse(result, null);
        }
    }
}
