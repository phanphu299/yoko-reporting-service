using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class ExportReportValidation : AbstractValidator<ExportReport>
    {
        public ExportReportValidation()
        {
            RuleFor(x => x.ObjectType).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Ids).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}