using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class FailedScheduleService : BaseSearchService<Domain.Entity.FailedSchedule, int, SearchFailedSchedule, FailedScheduleDto>, IFailedScheduleService
    {
        private readonly IReportService _reportService;
        private readonly IReportingUnitOfWork _reportingUnitOfWork;
        private readonly ILoggerAdapter<ReportService> _logger;

        public FailedScheduleService(IServiceProvider serviceProvider,
                            IReportService reportService,
                            IReportingUnitOfWork reportingUnitOfWOrk,
                            ILoggerAdapter<ReportService> logger)
             : base(FailedScheduleDto.Create, serviceProvider)
        {
            _reportService = reportService;
            _reportingUnitOfWork = reportingUnitOfWOrk;
            _logger = logger;
        }

        public async Task<IEnumerable<GenerateReportDto>> RunFailedScheduleAsync(RunFailedSchedule command)
        {
            if (string.IsNullOrWhiteSpace(command.JobId))
            {
                throw new EntityNotFoundException();
            }

            var failedSchedule = await _reportingUnitOfWork.FailedScheduleRepository.AsQueryable().AsNoTracking().Where(x => x.JobId == command.JobId).OrderByDescending(x => x.CreatedUtc).FirstOrDefaultAsync();
            if (failedSchedule == null)
            {
                throw new EntityNotFoundException();
            }

            var generateReportCommand = new GenerateReport(failedSchedule, command.Templates);
            var result = await _reportService.GenerateReportAsync(generateReportCommand);

            if (result == null)
            {
                throw new GenericProcessFailedException();
            }

            await _reportingUnitOfWork.BeginTransactionAsync();
            try
            {
                await _reportingUnitOfWork.FailedScheduleRepository.RemoveAsync(failedSchedule.Id);
                await _reportingUnitOfWork.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await _reportingUnitOfWork.RollbackAsync();
                throw;
            }

            return result;
        }

        protected override Type GetDbType()
        {
            return typeof(IFailedScheduleRepository);
        }
    }
}