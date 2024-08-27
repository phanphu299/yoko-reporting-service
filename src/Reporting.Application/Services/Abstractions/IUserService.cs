using System;
using System.Threading.Tasks;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IUserService
    {
        Task<ContactDto> FetchContactAsync(Guid contactId);
        Task<ContactGroupDto> FetchContactGroupAsync(Guid contactGroupId);
    }
}