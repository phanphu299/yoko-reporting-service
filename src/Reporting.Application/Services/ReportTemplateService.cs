using AHI.Infrastructure.Service;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using System;

namespace Reporting.Application.Service
{
    public class ReportTemplateService : BaseSearchService<Domain.Entity.ReportTemplate, long, SearchReportTemplate, ReportTemplateDto>, IReportTemplateService
    {
        public ReportTemplateService(IServiceProvider serviceProvider)
             : base(ReportTemplateDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IReportTemplateRepository);
        }
    }
}