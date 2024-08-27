using System.Threading.Tasks;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface ITemplateRepository : IRepository<Domain.Entity.Template, int>
    {
        Task<Domain.Entity.Template> PartialUpdateTemplateAsync(Domain.Entity.Template requestObject, Domain.Entity.Template targetObject);
        Task RemoveTemplatesAsync(IEnumerable<int> ids);
        Task RetrieveTemplateAsync(IEnumerable<Reporting.Domain.Entity.Template> templates);
        Task RetrieveTemplateDetailAsync(IEnumerable<Domain.Entity.TemplateDetail> details);
    }
}