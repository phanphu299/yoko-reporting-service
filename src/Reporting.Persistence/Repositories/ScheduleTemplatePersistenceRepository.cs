using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Persistence.Repository
{
    public class ScheduleTemplatePersistenceRepository : IScheduleTemplateRepository
    {
        private readonly ReportingDbContext _context;

        public ScheduleTemplatePersistenceRepository(ReportingDbContext context)
        {
            _context = context;
        }

        public async Task DeleteByScheduleIdAsync(int scheduleId)
        {
            var scheduleTemplates = await _context.ScheduleTemplates
                                                        .AsQueryable()
                                                        .Where(x => x.ScheduleId == scheduleId)
                                                        .ToListAsync();

            _context.ScheduleTemplates.RemoveRange(scheduleTemplates);
        }

        public Task<bool> IsBeingUsedAsync(IEnumerable<int> templateIds)
        {
            return _context.ScheduleTemplates.AnyAsync(x => templateIds.Contains(x.TemplateId));
        }
    }
}