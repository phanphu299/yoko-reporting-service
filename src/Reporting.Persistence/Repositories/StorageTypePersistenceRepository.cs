using System;
using System.Linq;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;

namespace Reporting.Persistence.Repository
{
    public class StorageTypePersistenceRepository : GenericRepository<Domain.Entity.StorageType, string>, IStorageTypeRepository
    {
        private readonly ReportingDbContext _context;

        public StorageTypePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.StorageType> AsQueryable() => _context.StorageTypes;

        protected override void Update(Domain.Entity.StorageType requestObject, Domain.Entity.StorageType targetObject)
        {
            throw new NotImplementedException();
        }
    }
}