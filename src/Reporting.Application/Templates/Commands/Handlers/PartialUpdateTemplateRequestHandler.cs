using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class PartialUpdateTemplateRequestHandler : IRequestHandler<PartialUpdateTemplate, TemplateDto>
    {
        private readonly ITemplateService _service;

        public PartialUpdateTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public Task<TemplateDto> Handle(PartialUpdateTemplate request, CancellationToken cancellationToken)
        {
            return _service.PartialUpdateTemplateAsync(request);
        }
    }
}