using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;
using System.Collections.Generic;
using System;

namespace Reporting.Persistence.Repository
{
    public class ReportPersistenceRepository : GenericRepository<Domain.Entity.Report, int>, IReportRepository
    {
        private readonly ReportingDbContext _context;

        public ReportPersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.Report> AsQueryable() => _context.Reports.Include(x => x.Template).Include(x => x.OutputType);

        protected override void Update(Domain.Entity.Report requestObject, Domain.Entity.Report targetObject)
        {
            targetObject.TemplateName = requestObject.TemplateName;
        }

        public Task UpdateTemplateNameOfReportListAsync(IEnumerable<Domain.Entity.Report> targetObjectList, string templateName)
        {
            foreach (var item in targetObjectList)
            {
                item.TemplateName = templateName;
            }
            return Task.CompletedTask;
        }

        public override Task<Domain.Entity.Report> FindAsync(int id)
        {
            return _context.Reports.Include(x => x.Template)
                                        .ThenInclude(x => x.OutputType)
                                    .Include(x => x.Template)
                                        .ThenInclude(x => x.Storage)
                                        .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}