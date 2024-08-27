using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class GenerateReportValidation : AbstractValidator<GenerateReport>
    {
        public GenerateReportValidation()
        {
            RuleFor(x => x.Templates)
                .NotEmpty()
                .When(x => x.TemplateId == null)
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}