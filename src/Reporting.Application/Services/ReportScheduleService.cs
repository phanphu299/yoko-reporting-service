using AHI.Infrastructure.Service;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using System;

namespace Reporting.Application.Service
{
    public class ReportScheduleService : BaseSearchService<Domain.Entity.ReportSchedule, int, SearchReportSchedule, ReportScheduleDto>, IReportScheduleService
    {
        public ReportScheduleService(IServiceProvider serviceProvider)
             : base(ReportScheduleDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IReportScheduleRepository);
        }
    }
}