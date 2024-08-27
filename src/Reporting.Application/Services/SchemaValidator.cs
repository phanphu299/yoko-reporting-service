using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using FluentValidation.Results;
using Reporting.Application.Command;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class SchemaValidator : ISchemaValidator
    {
        private readonly ISchemaService _schemaService;

        public SchemaValidator(ISchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        public async Task ValidateAsync(IEnumerable<BaseTemplateDetail> details)
        {
            if (details == null || !details.Any())
                return;

            foreach (var detail in details)
            {
                await ValidateAsync(detail.DataSourceTypeId, detail.DataSourceContent);
            }
        }

        public async Task ValidateAsync(string type, IDictionary<string, object> detail)
        {
            var schema = await _schemaService.GetByTypeAsync(new GetSchema(type));
            if (schema == null)
            {
                throw EntityValidationExceptionHelper.GenerateException("Type", ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
            var exceptions = new List<ValidationFailure>();
            foreach (var d in schema.Details.Where(x => x.IsRequired))
            {
                if (!detail.ContainsKey(d.Key))
                {
                    exceptions.Add(new ValidationFailure(d.Name, ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED));
                }
            }
            if (exceptions.Any())
            {
                throw EntityValidationExceptionHelper.GenerateException(exceptions);
            }
        }
    }
}