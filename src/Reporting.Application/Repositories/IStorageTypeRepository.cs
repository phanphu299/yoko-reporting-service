using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IStorageTypeRepository : IRepository<Domain.Entity.StorageType, string>
    {
    }
}