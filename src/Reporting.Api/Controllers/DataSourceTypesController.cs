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
    public class DataSourceTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DataSourceTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchDataSourceTypesAsync([FromBody] SearchDataSourceType command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}