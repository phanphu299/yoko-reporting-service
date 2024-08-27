using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Command;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class ReportAndSendScheduleValidationHandler : IScheduleValidationHandler
    {
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        public ReportAndSendScheduleValidationHandler(
            IReportingUnitOfWork reportingUnitOfWork
            )
        {
            _reportingUnitOfWork = reportingUnitOfWork;
        }

        public async Task HandleAsync<T>(T command) where T : IUpsertSchedule
        {
            await ValidateTemplateAsync(command.Templates);
        }

        private async Task ValidateTemplateAsync(IEnumerable<int> templates)
        {
            if (templates == null || !templates.Any())
                throw EntityValidationExceptionHelper.GenerateException(nameof(templates), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            var distinctIds = templates.Distinct().ToList();
            var entityCount = await _reportingUnitOfWork.TemplateRepository
                                                        .AsQueryable()
                                                        .AsNoTracking()
                                                        .CountAsync(x => distinctIds.Contains(x.Id));
            if (entityCount != distinctIds.Count)
            {
                throw EntityValidationExceptionHelper.GenerateException(nameof(templates), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_NOT_FOUND);
            }
        }
    }
}