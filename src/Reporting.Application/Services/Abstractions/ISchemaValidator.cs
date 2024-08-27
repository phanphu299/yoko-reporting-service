using System.Threading.Tasks;
using System.Collections.Generic;
using Reporting.Application.Command;

namespace Reporting.Application.Service.Abstraction
{
    public interface ISchemaValidator
    {
        Task ValidateAsync(IEnumerable<BaseTemplateDetail> details);
        Task ValidateAsync(string type, IDictionary<string, object> detail);
    }
}