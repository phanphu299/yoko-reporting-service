using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Persistence.Repository
{
    public class StoragePersistenceRepository : GenericRepository<Domain.Entity.Storage, int>, IStorageRepository
    {
        private readonly ReportingDbContext _context;

        public StoragePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.Storage> AsQueryable() => _context.Storages.Include(x => x.Type);

        protected override void Update(Domain.Entity.Storage requestObject, Domain.Entity.Storage targetObject)
        {
            if (!targetObject.CanEdit)
                throw new EntityInvalidException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_INVALID_NON_EDITABLE);

            targetObject.Name = requestObject.Name;
            targetObject.TypeId = requestObject.TypeId;
            targetObject.Content = requestObject.Content;
            targetObject.UpdatedUtc = requestObject.UpdatedUtc;
        }

        public override Task<Domain.Entity.Storage> FindAsync(int id)
        {
            return _context.Storages.Include(x => x.Type).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemoveStoragesAsync(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var e = await _context.Storages.FirstOrDefaultAsync(x => x.Id == id);
                if (e == null)
                {
                    throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
                }
                if (!e.CanDelete)
                {
                    throw new EntityInvalidException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_INVALID_PUBLISHING_NON_DELETEABLE);
                }
                e.Deleted = true;
                e.UpdatedUtc = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}