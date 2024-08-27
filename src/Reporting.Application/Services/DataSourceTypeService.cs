using System;
using AHI.Infrastructure.Service;
using Reporting.Application.Repository;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class DataSourceTypeService : BaseSearchService<Domain.Entity.DataSourceType, string, SearchDataSourceType, DataSourceTypeDto>, IDataSourceTypeService
    {
        public DataSourceTypeService(IServiceProvider serviceProvider)
             : base(DataSourceTypeDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IDataSourceTypeRepository);
        }
    }
}