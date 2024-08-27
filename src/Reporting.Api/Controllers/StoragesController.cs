using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Reporting.Application.Command;
using MediatR;
using Device.Application.Constant;
using AHI.Infrastructure.Authorization;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class StoragesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoragesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPost("search/type")]
        public async Task<IActionResult> SearchStorageTypesAsync([FromBody] SearchStorageType command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.ReportTemplate.FullRights.READ_REPORT_TEMPLATE)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchStoragesAsync([FromBody] SearchStorage command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpGet("{id}", Name = "GetStorage")]
        public async Task<IActionResult> GetStorageAsync([FromRoute] int id)
        {
            var command = new GetStorage(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPost]
        public async Task<IActionResult> AddStorageAsync([FromBody] AddStorage command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtRoute("GetStorage", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.WRITE_REPORT)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStorageAsync([FromRoute] int id, [FromBody] UpdateStorage command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return AcceptedAtRoute("GetStorage", new { id = response.Id }, response);
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.DELETE_REPORT)]
        [HttpDelete]
        public async Task<IActionResult> DeleteStoragesAsync([FromBody] IEnumerable<int> ids)
        {
            var command = new DeleteStorage(ids);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}