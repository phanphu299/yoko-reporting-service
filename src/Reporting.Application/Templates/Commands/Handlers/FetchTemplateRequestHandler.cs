using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class FetchTemplateRequestHandler : IRequestHandler<FetchTemplate, TemplateDto>
    {
        private readonly ITemplateService _service;

        public FetchTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public Task<TemplateDto> Handle(FetchTemplate request, CancellationToken cancellationToken)
        {
            return _service.FetchAsync(request.Id);
        }
    }
}