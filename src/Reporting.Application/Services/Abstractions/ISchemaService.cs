using System.Threading.Tasks;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface ISchemaService
    {
        Task<SchemaDto> GetByTypeAsync(GetSchema command);
    }
}