using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class GetDatasetFromTemplateFileValidation : AbstractValidator<GetDatasetFromTemplateFile>
    {
        public GetDatasetFromTemplateFileValidation()
        {
            RuleFor(x => x.TemplateFileUrl).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}