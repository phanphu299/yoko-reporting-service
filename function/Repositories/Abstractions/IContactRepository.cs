using System;
using System.Threading.Tasks;

namespace Reporting.Function.Service.Abstraction
{
    public interface IContactRepository
    {
        Task RemoveContactByProjectAsync(Guid objectId, string objectType);
        Task UpdateContactByProjectAsync(Guid objectId, string objectType, bool deleted);
    }
}
