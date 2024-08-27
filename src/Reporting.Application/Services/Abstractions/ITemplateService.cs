using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.Service.Abstraction;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using System.Threading;
using Reporting.Application.Template.Command;
using System.Collections.Generic;

namespace Reporting.Application.Service.Abstraction
{
    public interface ITemplateService : ISearchService<Domain.Entity.Template, int, SearchTemplate, TemplateDto>, IFetchService<Domain.Entity.Template, int, TemplateDto>
    {
        Task<TemplateByIdDto> GetTemplateByIdAsync(GetTemplateById command);
        Task<TemplateDto> AddTemplateAsync(AddTemplate command);
        Task<TemplateDto> PartialUpdateTemplateAsync(PartialUpdateTemplate command);
        Task<BaseResponse> DeleteTemplateAsync(DeleteTemplate command);
        Task<IEnumerable<TemplateBasicDto>> ArchiveAsync(ArchiveTemplate command, CancellationToken cancellationToken);
        Task<bool> VerifyArchiveAsync(VerifyArchivedTemplate command, CancellationToken cancellationToken);
        Task<bool> RetrieveAsync(RetrieveTemplate request, CancellationToken cancellationToken);
    }
}