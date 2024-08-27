using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using Reporting.Domain.Entity;

namespace Reporting.Application.Service
{
    public class ReportAndSendProcessor : IReportProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly ILoggerAdapter<ReportAndSendProcessor> _logger;

        public ReportAndSendProcessor(
            IServiceProvider serviceProvider, ITenantContext tenantContext, IUserContext userContext, ILoggerAdapter<ReportAndSendProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _tenantContext = tenantContext;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task ProcessAsync(ScheduleExecution execution, IExecutionParameter executionParameter)
        {
            var generatedReports = await ProcessReportAsync(execution.Id, execution.Schedule, executionParameter);
            executionParameter.AddRange(generatedReports);
        }

        private async Task<IEnumerable<GenerateReportDto>> ProcessReportAsync(Guid executionId, Domain.Entity.Schedule schedule, IExecutionParameter executionParameter)
        {
            var command = executionParameter.BuildRequest();
            
            var generateTime = command.ExtractFromDateToDate(command.TimeZoneName, command.DateTimeFormat);
            var distinctTemplates = command.Templates.Distinct();

            var generateTasks = new List<Task<GenerateReportDto>>();
            foreach (var templateId in distinctTemplates)
            {
                generateTasks.Add(GenerateReportAsync(executionId, templateId, command, generateTime, schedule));
            }

            // run generate report in parallel
            try
            {
                await Task.WhenAll(generateTasks);
            }
            catch
            {
                // Does not throw and wait for retry.
                foreach (var task in generateTasks.Where(task => task.IsFaulted && task.Exception != null))
                {
                    _logger.LogError(task.Exception, task.Exception.Message);
                }
            }

            var result = new List<GenerateReportDto>();
            foreach (var task in generateTasks.Where(task => task.IsCompletedSuccessfully))
            {
                result.Add(task.Result);
            }

            return result;
        }

        private async Task<GenerateReportDto> GenerateReportAsync(Guid executionId, int templateId, GenerateReport command, GenerateReport.GenerateTime generateTime, Domain.Entity.Schedule schedule)
        {
            using (var scope = _serviceProvider.CreateScope(_tenantContext, _userContext))
            {
                var reportGenerator = scope.ServiceProvider.GetRequiredService<ReportGenerator>();
                return await reportGenerator.GenerateReportAsync(executionId, templateId, command, generateTime, schedule);
            }
        }
    }
}