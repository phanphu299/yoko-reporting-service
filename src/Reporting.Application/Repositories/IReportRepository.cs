using AHI.Infrastructure.Repository.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Repository
{
    public interface IReportRepository : IRepository<Domain.Entity.Report, int>
    {
        Task UpdateTemplateNameOfReportListAsync(IEnumerable<Domain.Entity.Report> targetObjectList, string templateName);
    }
}