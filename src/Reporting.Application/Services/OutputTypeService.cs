using System;
using AHI.Infrastructure.Service;
using Reporting.Application.Repository;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class OutputTypeService : BaseSearchService<Domain.Entity.OutputType, string, SearchOutputType, OutputTypeDto>, IOutputTypeService
    {
        public OutputTypeService(IServiceProvider serviceProvider)
             : base(OutputTypeDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IOutputTypeRepository);
        }
    }
}