using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Reporting.Application.Command;
using MediatR;
using Device.Application.Constant;
using AHI.Infrastructure.Authorization;
using Reporting.Application.Template.Command;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class TemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchTemplatesAsync([FromBody] SearchTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpGet("{id}", Name = "GetTemplate")]
        public async Task<IActionResult> GetTemplateAsync([FromRoute] int id)
        {
            var command = new GetTemplateById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.WRITE_REPORT_TEMPLATE)]
        [HttpPost]
        public async Task<IActionResult> AddTemplateAsync([FromBody] AddTemplate command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtRoute("GetTemplate", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.WRITE_REPORT_TEMPLATE)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateTemplateAsync([FromRoute] int id, [FromBody] JsonPatchDocument patchDocument)
        {
            var command = new PartialUpdateTemplate(id, patchDocument);
            var response = await _mediator.Send(command);
            return AcceptedAtRoute("GetTemplate", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.DELETE_REPORT_TEMPLATE)]
        [HttpDelete]
        public async Task<IActionResult> DeleteTemplatesAsync([FromBody] IEnumerable<int> ids)
        {
            var command = new DeleteTemplate(ids);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpPost("datasets")]
        public async Task<IActionResult> GetDataSetsAsync([FromBody] GetDatasetFromTemplateFile command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}/fetch")]
        public async Task<IActionResult> FetchAsync(int id)
        {
            var command = new FetchTemplate(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        
        [HttpPost("archive")]
        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        public async Task<IActionResult> ArchiveAsync([FromBody] ArchiveTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("archive/verify")]
        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        public async Task<IActionResult> VerifyArchiveAsync([FromBody] VerifyArchivedTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("retrieve")]
        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.WRITE_REPORT_TEMPLATE)]
        public async Task<IActionResult> RetrieveAsync([FromBody] RetrieveTemplate command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}