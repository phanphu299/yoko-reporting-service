using System.Linq;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;

namespace Reporting.Persistence.Repository
{
    public class SchemaPersistenceRepository : ISchemaRepository
    {
        private readonly ReportingDbContext _context;

        public SchemaPersistenceRepository(ReportingDbContext context)
        {
            _context = context;
        }

        public IQueryable<Domain.Entity.Schema> AsQueryable() => _context.Schemas;
    }
}