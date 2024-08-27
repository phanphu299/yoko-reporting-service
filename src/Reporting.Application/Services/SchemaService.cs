using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Command.Model;
using Reporting.Application.Command;

namespace Reporting.Application.Service
{
    public class SchemaService : ISchemaService
    {
        private readonly ISchemaRepository _repository;

        public SchemaService(ISchemaRepository repository)
        {
            _repository = repository;
        }

        public async Task<SchemaDto> GetByTypeAsync(GetSchema command)
        {
            var schema = await _repository.AsQueryable().Include(x => x.Details).FirstOrDefaultAsync(x => x.Type == command.Type);
            return SchemaDto.Create(schema);
        }
    }
}