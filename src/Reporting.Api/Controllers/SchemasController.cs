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
    public class SchemasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchemasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetSchemaByTypeAsync([FromRoute] string type)
        {
            var command = new GetSchema(type);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}