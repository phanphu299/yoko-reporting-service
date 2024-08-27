using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class UpdateStorageValidation : AbstractValidator<UpdateStorage>
    {
        public UpdateStorageValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.TypeId).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Content).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}