using System;
using AHI.Infrastructure.Service;
using Reporting.Application.Repository;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class ScheduleTypeService : BaseSearchService<Domain.Entity.ScheduleType, string, SearchScheduleType, ScheduleTypeDto>, IScheduleTypeService
    {
        public ScheduleTypeService(IServiceProvider serviceProvider)
             : base(ScheduleTypeDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IScheduleTypeRepository);
        }
    }
}