using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Command;

namespace Reporting.Application.Validation
{
    public class DeleteScheduleValidation : AbstractValidator<DeleteSchedule>
    {
        public DeleteScheduleValidation()
        {
            RuleFor(x => x.Ids).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}