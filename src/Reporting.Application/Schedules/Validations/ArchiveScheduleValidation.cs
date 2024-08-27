using AHI.Infrastructure.Exception;
using FluentValidation;
using Reporting.Application.Extension;
using Reporting.Application.Command.Model;
using Scheduler.Application.Helper;
using Reporting.Application.Constant;

namespace Reporting.Application.Schedule.Validation
{
    public class ArchiveScheduleValidation : AbstractValidator<ScheduleDto>
    {
        public ArchiveScheduleValidation()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Templates).NotEmpty().When(x => x.Type == ScheduleType.REPORT_AND_SEND).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Type).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.Cron).Must(CronJobHelper.IsValidCronExpression).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.TimeZoneName).Must(IsValidTimezone).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }

        public bool IsValidTimezone(string timeZoneName)
        {
            var timeZoneInfo = timeZoneName.GetTimeZoneInfo();
            return timeZoneInfo != null;
        }
    }
}
