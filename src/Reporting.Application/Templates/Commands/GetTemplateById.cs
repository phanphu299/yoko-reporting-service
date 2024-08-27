using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetTemplateById : IRequest<TemplateByIdDto>
    {
        public int Id { get; set; }

        public GetTemplateById(int id)
        {
            Id = id;
        }
    }
}