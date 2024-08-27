using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Event;
using Reporting.Application.Extension;
using Reporting.Application.Notification;
using Reporting.Application.Repository;
using Reporting.Application.Service.Abstraction;
using Reporting.Domain.Entity;
using Reporting.Domain.EnumType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IReportingUnitOfWork _unitOfWork;
        private readonly ITenantContext _tenantContext;
        private readonly IReportNotifyContentBuilder _reportNotifyContentBuilder;
        private readonly WhatsAppTemplateOptions _whatsappTemplateOptions;
        private readonly ILoggerAdapter<NotificationHandler> _logger;

        public NotificationHandler(
            IDomainEventDispatcher dispatcher,
            IReportingUnitOfWork unitOfWork,
            ITenantContext tenantContext,
            IReportNotifyContentBuilder reportNotifyContentBuilder,
            WhatsAppTemplateOptions whatsappTemplateOptions,
            ILoggerAdapter<NotificationHandler> logger
            )
        {
            _dispatcher = dispatcher;
            _unitOfWork = unitOfWork;
            _tenantContext = tenantContext;
            _reportNotifyContentBuilder = reportNotifyContentBuilder;
            _whatsappTemplateOptions = whatsappTemplateOptions;
            _logger = logger;
        }

        public async Task SendGeneratedReportsAsync(ScheduleExecution execution, ReportAndSendParameter parameters)
        {
            var contacts = await _unitOfWork.ScheduleContactRepository.GetScheduleContactsByScheduleIdAsync(execution.ScheduleId);
            if (contacts != null && contacts.Any())
            {
                var reportIds = parameters.Generated.Select(x => x.ReportId).ToArray();
                var reports = await _unitOfWork.ReportRepository.AsQueryable().AsNoTracking().Where(x => reportIds.Contains(x.Id)).ToListAsync();
                var zipUrl = await _reportNotifyContentBuilder.CreateZipReportsAsync(execution, parameters.Request.TimeZoneName, reports.ToList());
                _logger.LogInformation("Generated report zip file for schedule {ScheduleId}, execution {ScheduleExecutionId}, URL: '{Url}'",
                    execution.ScheduleId, execution.Id, zipUrl);

                var messages = await BuildGeneratedReportsMessagesAsync(execution, parameters, zipUrl);

                var listContactSimple = contacts.Select(contact => ScheduleContactSimpleDto.CreateDto(contact));

                var reportTriggeredEvent = new ScheduleTriggeredEvent(messages, listContactSimple, _tenantContext, execution.ScheduleId.ToString());
                await _dispatcher.SendAsync(reportTriggeredEvent);
                _logger.LogInformation("Sent notification message for schedule {ScheduleId}, execution {ScheduleExecutionId}, URL: '{Url}'",
                    execution.ScheduleId, execution.Id, zipUrl);
            }
        }

        private async Task<IDictionary<string, BaseNotifyMessage>> BuildGeneratedReportsMessagesAsync(
            ScheduleExecution execution, ReportAndSendParameter parameters, string zipUrl)
        {
            var content = ReportNotificationTemplates.MainContent;
            if (parameters.State == ExecutionState.PFIN)
            {
                content += ReportNotificationTemplates.ReportAndSendScheduleFailedList;
            }
            if (!string.IsNullOrEmpty(zipUrl))
            {
                content += ReportNotificationTemplates.DownloadLinkContent;
            }

            var payload = new Dictionary<string, object>()
                {
                    { "scheduleName", execution.Schedule.Name },
                    { "timestamp", $"{DateTime.UtcNow.ToLocalDateTime(parameters.Request.TimeZoneName).ToString(DateTimeExtension.LONG_TIMESTAMP_FORMAT)}" },
                    { "state", parameters.State.GetString() },
                    { "url", zipUrl },
                };

            var generatedTemplateIds = parameters.Generated.Select(x => x.TemplateId);
            var failedTemplateIds = parameters.Request.Templates.Except(generatedTemplateIds).Select(x => (long)x);
            var failedTemplateNames = await _unitOfWork.TemplateRepository
                                                .AsQueryable()
                                                .AsNoTracking()
                                                .Where(x => failedTemplateIds.Contains(x.Id))
                                                .Select(x => x.Name)
                                                .ToListAsync();

            payload["failedCount"] = parameters.TargetCount - parameters.TotalGenerated;
            payload["failedList"] = string.Join("<br>", failedTemplateNames.Select(name => $" + {name}"));

            var whatsAppTemplate = parameters.State switch
            {
                ExecutionState.PFIN => _whatsappTemplateOptions.ReportAndSendSchedulePartial,
                ExecutionState.FIN => _whatsappTemplateOptions.ReportSchedulesSuccess,
                _ => _whatsappTemplateOptions.ReportSchedulesFail
            };

            var whatsAppPayload = new Dictionary<string, object>(payload);
            whatsAppPayload["scheduleName"] = (whatsAppPayload["scheduleName"] as string).Replace("\\", "\\\\").Replace("\"", "\\\"");
            whatsAppPayload["failedList"] = string.Join("; ", failedTemplateNames);

            return CreateMessages(content, payload, whatsAppTemplate, whatsAppPayload);
        }

        public async Task SendCollectedReportsAsync(ScheduleExecution execution, CollectReportsParameters parameters,
            List<Report> reports, List<string> failedSchedules, List<string> partialSuccessSchedules)
        {
            var zipUrl = await _reportNotifyContentBuilder.CreateZipReportsAsync(execution, parameters.TimeZoneName, reports);
            _logger.LogInformation("Generated report zip file for schedule {ScheduleId}, execution {ScheduleExecutionId}, URL: '{Url}'",
                execution.ScheduleId, execution.Id, zipUrl);

            var contacts = await _unitOfWork.ScheduleContactRepository.GetScheduleContactsByScheduleIdAsync(execution.ScheduleId);
            if (contacts != null && contacts.Any())
            {
                var messages = await BuildCollectedReportsMessagesAsync(execution, parameters, zipUrl,
                    failedSchedules, partialSuccessSchedules);

                var listContactSimple = contacts.Select(contact => ScheduleContactSimpleDto.CreateDto(contact));

                var reportTriggeredEvent = new ScheduleTriggeredEvent(messages, listContactSimple, _tenantContext, execution.ScheduleId.ToString());
                await _dispatcher.SendAsync(reportTriggeredEvent);
                _logger.LogInformation("Sent notification message for schedule {ScheduleId}, execution {ScheduleExecutionId}, URL: '{Url}'",
                    execution.ScheduleId, execution.Id, zipUrl);
            }
        }

        private async Task<IDictionary<string, BaseNotifyMessage>> BuildCollectedReportsMessagesAsync(
            ScheduleExecution execution, CollectReportsParameters parameters, string zipUrl,
            List<string> failedSchedules, List<string> partialSuccessSchedules)
        {
            var content = ReportNotificationTemplates.MainContent;
            if (execution.State == ExecutionState.PFIN)
            {
                content += ReportNotificationTemplates.SendScheduleIssueCount;
                if (failedSchedules.Count > 0)
                {
                    content += ReportNotificationTemplates.SendScheduleFailedList;
                }
                if (partialSuccessSchedules.Count > 0)
                {
                    content += ReportNotificationTemplates.SendSchedulePartialSuccessList;
                }
            }
            if (!string.IsNullOrEmpty(zipUrl))
            {
                content += ReportNotificationTemplates.DownloadLinkContent;
            }

            var payload = new Dictionary<string, object>()
                {
                    { "scheduleName", execution.Schedule.Name },
                    { "timestamp", $"{DateTime.UtcNow.ToLocalDateTime(parameters.TimeZoneName).ToString(DateTimeExtension.LONG_TIMESTAMP_FORMAT)}" },
                    { "state", execution.State.GetString() },
                    { "url", zipUrl },
                };

            payload["issueCount"] = failedSchedules.Count + partialSuccessSchedules.Count;
            payload["failedCount"] = failedSchedules.Count;
            payload["failedList"] = string.Join("<br>", failedSchedules.Select(name => $" - {name}"));
            payload["partialSuccessCount"] = partialSuccessSchedules.Count;
            payload["partialSuccessList"] = string.Join("<br>", partialSuccessSchedules.Select(name => $" - {name}"));

            var whatsAppTemplate = execution.State switch
            {
                ExecutionState.PFIN => failedSchedules.Count == 0 ? _whatsappTemplateOptions.SendSchedulePartial
                    : partialSuccessSchedules.Count == 0 ? _whatsappTemplateOptions.SendSchedulePartialFailed
                    : _whatsappTemplateOptions.SendSchedulePartialMixed,
                ExecutionState.FIN => _whatsappTemplateOptions.ReportSchedulesSuccess,
                _ => _whatsappTemplateOptions.ReportSchedulesFail
            };

            var whatsAppPayload = new Dictionary<string, object>(payload);
            whatsAppPayload["scheduleName"] = (whatsAppPayload["scheduleName"] as string).Replace("\\", "\\\\").Replace("\"", "\\\"");
            whatsAppPayload["failedList"] = string.Join("; ", failedSchedules);
            whatsAppPayload["partialSuccessList"] = string.Join("; ", partialSuccessSchedules);

            return CreateMessages(content, payload, whatsAppTemplate, whatsAppPayload);
        }

        private Dictionary<string, BaseNotifyMessage> CreateMessages(string content, object payload,
            WhatsAppTemplateDefinition whatsAppTemplate, object whatsAppPayload)
        {
            return new Dictionary<string, BaseNotifyMessage>
            {
                {
                    SchemaTypeCodeConstants.EMAIL,
                    new EmailNotifyMessage(
                        content + ReportNotificationTemplates.EmailSignOff,
                        ReportNotificationTemplates.EmailSubject,
                        true,
                        payload)
                },
                {
                    SchemaTypeCodeConstants.WEB_HOOK,
                    new WebHookNotifyMessage(
                        content,
                        payload)
                },
                {
                    SchemaTypeCodeConstants.WHATSAPP,
                    new WhatsAppNotifyMessage(
                        $"{{{string.Join(",", whatsAppTemplate.Parameters.Select((x, i) => $"\"{i+1}\":\"{{{{{x}}}}}\""))}}}",
                        whatsAppTemplate.Id,
                        whatsAppPayload)
                }
            };
        }
    }
}
