using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Reporting.Application.Command;
using MediatR;
using AHI.Infrastructure.Authorization;
using Device.Application.Constant;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class ScheduleTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RightsAuthorizeFilter(Privileges.Report.FullRights.READ_REPORT)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchScheduleTypesAsync([FromBody] SearchScheduleType command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}