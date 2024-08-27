using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IDataSourceHandler
    {
        Task<IEnumerable<DataTableResult>> HandleAsync(GetDataSource command);
        Task<Dictionary<string, string>> CheckExistDataSourceAsync(CheckExistDataSource command);
    }
}