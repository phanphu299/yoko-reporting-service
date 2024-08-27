using AHI.Infrastructure.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IDataSourceTypeService : ISearchService<Domain.Entity.DataSourceType, string, SearchDataSourceType, DataSourceTypeDto>
    {
    }
}