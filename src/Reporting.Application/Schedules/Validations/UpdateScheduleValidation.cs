using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Service.Tag.Model;
using FluentValidation;
using Reporting.Application.Command;
using Reporting.Application.Constant;

namespace Reporting.Application.Validation
{
    public class UpdateScheduleValidation : AbstractValidator<UpdateSchedule>
    {
        public UpdateScheduleValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.Templates)
                .NotEmpty()
                .When(x => x.Type == ScheduleType.REPORT_AND_SEND)
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.Cron)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.TimeZoneName)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);


            RuleFor(x => x.Period)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                .Matches(RegexConstants.SCHEDULE_PERIOD)
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID)
                .When(x => x.Type == ScheduleType.SEND);

            RuleFor(x => x.Jobs)
                .NotEmpty()
                .When(x => x.Type == ScheduleType.SEND)
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleForEach(x => x.Tags).SetValidator(
                new InlineValidator<UpsertTag>
                {
                                agValidator => agValidator.RuleFor(x => x.Key)
                                                        .NotEmpty()
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                                                        .MaximumLength(216)
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                                                        .Must(ContainsInvalidChar)
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID),
                                agValidator => agValidator.RuleFor(x => x.Value)
                                                        .NotEmpty()
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                                                        .MaximumLength(216)
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                                                        .Must(ContainsInvalidChar)
                                                        .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID)
                }
            );
        }

        private bool ContainsInvalidChar(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;
            return !input.Contains(':') && !input.Contains(';') && !input.Contains(',');
        }
    }
}