using System.Collections.Generic;

namespace Reporting.Application.Service.Abstraction
{
    public interface IDynamicProcessor
    {
        object Process(string expression, IDictionary<string, object> request);
    }
}