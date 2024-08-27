using System.Threading.Tasks;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IStorageRepository : IRepository<Domain.Entity.Storage, int>
    {
        Task RemoveStoragesAsync(IEnumerable<int> ids);
    }
}