using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IReportTemplateRepository : ISearchRepository<Domain.Entity.ReportTemplate, long>
    {
    }
}