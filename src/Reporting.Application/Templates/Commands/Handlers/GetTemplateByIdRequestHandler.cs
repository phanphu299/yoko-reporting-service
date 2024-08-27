using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class GetTemplateByIdRequestHandler : IRequestHandler<GetTemplateById, TemplateByIdDto>
    {
        private readonly ITemplateService _service;

        public GetTemplateByIdRequestHandler(ITemplateService service)
        {
            _service = service;
        }

        public Task<TemplateByIdDto> Handle(GetTemplateById request, CancellationToken cancellationToken)
        {
            return _service.GetTemplateByIdAsync(request);
        }
    }
}