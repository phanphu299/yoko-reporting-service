using AHI.Infrastructure.Repository.Generic;
using Reporting.Application.Repository;
using Reporting.Persistence.Context;
using System.Linq;

namespace Reporting.Persistence.Repository
{
    public class ReportSchedulePersistenceRepository : ISearchRepository<Domain.Entity.ReportSchedule, int>, IReportScheduleRepository
    {
        private readonly ReportingDbContext _context;

        public ReportSchedulePersistenceRepository(ReportingDbContext context)
        {
            _context = context;
        }

        public IQueryable<Domain.Entity.ReportSchedule> AsQueryable() => _context.ReportSchedules;
    }
}