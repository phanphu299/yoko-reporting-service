using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reporting.Application.Command;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class SendScheduleValidationHandler : IScheduleValidationHandler
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly IConfiguration _configuration;
        public SendScheduleValidationHandler(
            IReportingUnitOfWork reportingUnitOfWork,
            IConfiguration configuration
        )
        {
            _reportingUnitOfWork = reportingUnitOfWork;
            _configuration = configuration;
        }

        public async Task HandleAsync<T>(T command) where T : IUpsertSchedule
        {
            ValidatePeriod(command.Period);
            await ValidateJobAsync(command.Jobs);
        }

        private async Task ValidateJobAsync(IEnumerable<int> jobs)
        {
            if (jobs == null || !jobs.Any())
                throw EntityValidationExceptionHelper.GenerateException(nameof(jobs), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            var distinctIds = jobs.Distinct().ToList();
            var entityCount = await _reportingUnitOfWork.ScheduleRepository
                                                        .AsQueryable()
                                                        .AsNoTracking()
                                                        .CountAsync(x => distinctIds.Contains(x.Id) &&
                                                                         x.Type == ScheduleType.REPORT_AND_SEND);
            if (entityCount != distinctIds.Count)
            {
                throw EntityValidationExceptionHelper.GenerateException(nameof(jobs), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
        }

        private void ValidatePeriod(string period)
        {
            if (string.IsNullOrEmpty(period))
                throw EntityValidationExceptionHelper.GenerateException(nameof(period), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            if (!Regex.IsMatch(period, RegexConstants.SCHEDULE_PERIOD))
                throw EntityValidationExceptionHelper.GenerateException(nameof(period), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);

            var periodValue = StringExtension.ExtractPeriodValue(period);
            if (!long.TryParse(periodValue, out var value))
            {
                throw EntityValidationExceptionHelper.GenerateException(nameof(period), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
            }
            var unit = period.LastOrDefault().ToString();
            var yearValue = ConvertPeriodToYear(value, unit);

            var maxYear = int.TryParse(_configuration["MaxPeriodYears"], out var maxValue) ? maxValue : 3;
            if (yearValue > maxYear)
                throw EntityValidationExceptionHelper.GenerateException(nameof(period), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);

        }

        private double ConvertPeriodToYear(long value, string unit)
        {
            return unit switch
            {
                "y" => value,
                "M" => (double)value / 12,
                "d" => (double)value / 365,
                "h" => (double)value / 8765.81277,
                "m" => (double)value / 525948766,
                _ => value
            };
        }
    }
}