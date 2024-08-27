using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;
using System;

namespace Reporting.Application.Validation
{
    public class PreviewReportValidation : AbstractValidator<PreviewReport>
    {
        public PreviewReportValidation()
        {
            RuleFor(x => x.TemplateId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.FromDate).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.ToDate).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
            RuleFor(x => x.ToDate).Must(IsLessThanOrEqualsToCurrentDateTime).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
        }

        public bool IsLessThanOrEqualsToCurrentDateTime(DateTime time)
        {
            return time <= DateTime.UtcNow;
        }
    }
}