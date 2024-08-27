using System;
using System.Linq;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;

namespace Reporting.Persistence.Repository
{
    public class OutputTypePersistenceRepository : GenericRepository<Domain.Entity.OutputType, string>, IOutputTypeRepository
    {
        private readonly ReportingDbContext _context;

        public OutputTypePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.OutputType> AsQueryable() => _context.OutputTypes;

        protected override void Update(Domain.Entity.OutputType requestObject, Domain.Entity.OutputType targetObject)
        {
            throw new NotImplementedException();
        }
    }
}