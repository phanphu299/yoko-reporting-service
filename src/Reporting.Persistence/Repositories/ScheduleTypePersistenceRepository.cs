using System;
using System.Linq;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;

namespace Reporting.Persistence.Repository
{
    public class ScheduleTypePersistenceRepository : GenericRepository<Domain.Entity.ScheduleType, string>, IScheduleTypeRepository
    {
        private readonly ReportingDbContext _context;

        public ScheduleTypePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.ScheduleType> AsQueryable() => _context.ScheduleTypes;

        protected override void Update(Domain.Entity.ScheduleType requestObject, Domain.Entity.ScheduleType targetObject)
        {
            throw new NotImplementedException();
        }
    }
}