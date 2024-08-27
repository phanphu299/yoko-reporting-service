using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Reporting.Application.Command;
using MediatR;

namespace Reporting.Api.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class OutputTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OutputTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchOutputTypesAsync([FromBody] SearchOutputType command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}