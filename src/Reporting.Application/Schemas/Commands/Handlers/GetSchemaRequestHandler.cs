using System.Threading;
using System.Threading.Tasks;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command.Handler
{
    public class GetSchemaRequestHandler : IRequestHandler<GetSchema, SchemaDto>
    {
        private readonly ISchemaService _service;

        public GetSchemaRequestHandler(ISchemaService service)
        {
            _service = service;
        }

        public Task<SchemaDto> Handle(GetSchema request, CancellationToken cancellationToken)
        {
            return _service.GetByTypeAsync(request);
        }
    }
}