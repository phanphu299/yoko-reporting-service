using AHI.Infrastructure.Repository.Generic;
using Reporting.Application.Repository;
using Reporting.Persistence.Context;
using System.Linq;

namespace Reporting.Persistence.Repository
{
    public class ReportTemplatePersistenceRepository : ISearchRepository<Domain.Entity.ReportTemplate, long>, IReportTemplateRepository
    {
        private readonly ReportingDbContext _context;

        public ReportTemplatePersistenceRepository(ReportingDbContext context)
        {
            _context = context;
        }

        public IQueryable<Domain.Entity.ReportTemplate> AsQueryable() => _context.ReportTemplates;
    }
}