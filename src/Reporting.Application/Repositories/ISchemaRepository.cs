using System.Linq;

namespace Reporting.Application.Repository
{
    public interface ISchemaRepository
    {
        IQueryable<Domain.Entity.Schema> AsQueryable();
    }
}