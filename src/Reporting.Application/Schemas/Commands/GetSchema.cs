using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetSchema : IRequest<SchemaDto>
    {
        public string Type { get; set; }

        public GetSchema(string type)
        {
            Type = type;
        }
    }
}