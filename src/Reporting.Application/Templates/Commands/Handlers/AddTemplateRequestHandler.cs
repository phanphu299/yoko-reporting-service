using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class AddTemplateRequestHandler : IRequestHandler<AddTemplate, TemplateDto>
    {
        private readonly ITemplateService _service;

        public AddTemplateRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public Task<TemplateDto> Handle(AddTemplate request, CancellationToken cancellationToken)
        {
            return _service.AddTemplateAsync(request);
        }
    }
}