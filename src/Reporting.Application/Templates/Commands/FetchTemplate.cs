using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class FetchTemplate : IRequest<TemplateDto>
    {
        public int Id { get; set; }

        public FetchTemplate(int id)
        {
            Id = id;
        }
    }
}