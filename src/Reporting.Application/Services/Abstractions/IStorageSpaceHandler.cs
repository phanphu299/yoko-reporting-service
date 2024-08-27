using System.Threading.Tasks;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IStorageSpaceHandler
    {
        Task<UploadStorageSpaceDto> HandleAsync(UploadStorageSpace command);
        Task<string> GetDownloadFileUrlAsync(GetDownloadFileUrl command);
    }
}