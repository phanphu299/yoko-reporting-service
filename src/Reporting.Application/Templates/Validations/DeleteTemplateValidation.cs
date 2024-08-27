using Reporting.Application.Command;
using FluentValidation;
using AHI.Infrastructure.Exception;

namespace Reporting.Application.Validation
{
    public class DeleteTemplateValidation : AbstractValidator<DeleteTemplate>
    {
        public DeleteTemplateValidation()
        {
            RuleFor(x => x.Ids).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}