using AHI.Infrastructure.Authorization;
using AHI.Infrastructure.SharedKernel.Extension;
using Device.Application.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Command;
using Reporting.Application.Service.Abstraction;
using System;
using System.Threading.Tasks;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INativeStorageService _storageService;

        public ReportsController(IMediator mediator, INativeStorageService storageService)
        {
            _mediator = mediator;
            _storageService = storageService;
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchReportsAsync([FromBody] SearchReport command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("templates/search")]
        public async Task<IActionResult> SearchReportTemplatesAsync([FromBody] SearchReportTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("schedules/search")]
        public async Task<IActionResult> SearchReportSchedulesAsync([FromBody] SearchReportSchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpGet("{id}", Name = "GetReport")]
        public async Task<IActionResult> GetReportAsync([FromRoute] int id)
        {
            var command = new GetReport(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewReportAsync([FromBody] PreviewReport command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReportAsync([FromBody] GenerateReport command, [FromQuery] string job_id, [FromQuery] string time_zone_name, [FromQuery] long execution_time, [FromQuery] long next_execution_time, [FromQuery] long previous_execution_time)
        {
            command.JobId = job_id;
            command.TimeZoneName = time_zone_name;
            command.ExecutionTime = execution_time;
            command.NextExecutionTime = next_execution_time;
            command.PreviousExecutionTime = previous_execution_time;

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("executions/{executionId}/retry")]
        public async Task<IActionResult> RetryGenerateReportAsync([FromRoute] Guid executionId, [FromQuery] string job_id)
        {
            var command = new TriggerGenerateReport(executionId, job_id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("collect")]
        public async Task<IActionResult> TriggerCollectReportsAsync([FromBody] TriggerCollectReports command, [FromQuery] string job_id, [FromQuery] string time_zone_name, [FromQuery] long execution_time)
        {
            command.JobId = job_id;
            command.TimeZoneName = time_zone_name;
            command.ExecutionTimeUtc = execution_time.ToString().UnixTimeStampToDateTime();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("collect/retry")]
        public async Task<IActionResult> TriggerRetryCollectReportsAsync([FromBody] TriggerRetryCollectReports command, [FromQuery] string job_id)
        {
            command.JobId = job_id;
            await _mediator.Send(command);
            return Ok();
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpGet("{id}/download/url")]
        public async Task<IActionResult> GetDownloadFileUrlAsync([FromRoute] int id)
        {
            var command = new GetDownloadFileUrl(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("export")]
        public async Task<IActionResult> DownloadReportsAsync([FromBody] ExportReport command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("templates/export")]
        public async Task<IActionResult> DownloadReportsByTemplateAsync([FromBody] ExportReportByTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("schedules/export")]
        public async Task<IActionResult> DownloadReportsBySchedulesAsync([FromBody] ExportReportBySchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpPost("preview/export")]
        public async Task<IActionResult> ExportPreviewReportAsync([FromBody] ExportPreviewReport command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}