using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Service.Abstraction
{
    public interface IStorageService : ISearchService<Domain.Entity.Storage, int, SearchStorage, StorageDto>
    {
        Task<StorageDto> GetStorageByIdAsync(GetStorage command);
        Task<StorageDto> AddStorageAsync(AddStorage command);
        Task<StorageDto> UpdateStorageAsync(UpdateStorage command);
        Task<BaseResponse> DeleteStorageAsync(DeleteStorage command);
        Task<string> ExtractZipEntryAsync(string filePath, string contentPath, string folderPath);
    }
}