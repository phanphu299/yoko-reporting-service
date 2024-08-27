using System;
using System.Linq;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;

namespace Reporting.Persistence.Repository
{
    public class DataSourceTypePersistenceRepository : GenericRepository<Domain.Entity.DataSourceType, string>, IDataSourceTypeRepository
    {
        private readonly ReportingDbContext _context;

        public DataSourceTypePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.DataSourceType> AsQueryable() => _context.DataSourceTypes;

        protected override void Update(Domain.Entity.DataSourceType requestObject, Domain.Entity.DataSourceType targetObject)
        {
            throw new NotImplementedException();
        }
    }
}