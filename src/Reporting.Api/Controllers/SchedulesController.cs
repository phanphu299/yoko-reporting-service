using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Reporting.Application.Command;
using MediatR;
using Device.Application.Constant;
using AHI.Infrastructure.Authorization;
using Reporting.Application.Schedule.Command;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchSchedulesAsync([FromBody] SearchSchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpGet("{id}", Name = "GetSchedule")]
        public async Task<IActionResult> GetScheduleAsync([FromRoute] int id)
        {
            var command = new GetScheduleById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT, Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPost]
        public async Task<IActionResult> AddScheduleAsync([FromBody] AddSchedule command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtRoute("GetSchedule", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT, Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScheduleAsync([FromRoute] int id, [FromBody] UpdateSchedule command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return AcceptedAtRoute("GetSchedule", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT, Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateScheduleAsync([FromRoute] int id, [FromBody] JsonPatchDocument patchDocument)
        {
            var command = new PartialUpdateSchedule(id, patchDocument);
            var response = await _mediator.Send(command);
            return AcceptedAtRoute("GetSchedule", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.DELETE_REPORT)]
        [HttpDelete]
        public async Task<IActionResult> DeleteScheduleAsync([FromBody] IEnumerable<int> ids)
        {
            var command = new DeleteSchedule(ids);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("rerun")]
        public async Task<IActionResult> RerunFailedScheduleAsync([FromBody] RunFailedSchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("archive")]
        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        public async Task<IActionResult> ArchiveAsync([FromBody] ArchiveSchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("archive/verify")]
        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        public async Task<IActionResult> VerifyArchiveAsync([FromBody] VerifySchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("retrieve")]
        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        public async Task<IActionResult> RetrieveAsync([FromBody] RetrieveSchedule command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}