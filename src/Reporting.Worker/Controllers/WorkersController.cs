using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Extension;
using Reporting.Application.Service;
using Reporting.Worker.Service.Abstraction;
using System.Threading.Tasks;

namespace Reporting.Worker.Controller
{
    [Route("rpt/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class WorkersController : ControllerBase
    {
        private readonly IReportBuildingService _reportBuildingService;
        private readonly WorkerBackgroundService _backgroundService;
        private readonly IMemoryCache _memoryCache;

        public WorkersController(IReportBuildingService reportBuildingService, WorkerBackgroundService backgroundService, IMemoryCache memoryCache)
        {
            _reportBuildingService = reportBuildingService;
            _backgroundService = backgroundService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Build report in the foreground, the caller needs to wait
        /// </summary>
        [HttpPost("build")]
        public async Task<IActionResult> BuildReportFileAsync([FromBody] BuildReportFile command)
        {
            var filledStream = await _reportBuildingService.BuildReportFileAsync(command);
            return File(filledStream, command.Template.OutputType.Extension.GetContentType(), $"{command.Template.Name}{command.Template.OutputType.Extension}");
        }

        /// <summary>
        /// Build report in the background, the caller doesn't need to wait
        /// </summary>
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewReportFileAsync([FromBody] PreviewReportFile command)
        {
            await _backgroundService.QueueAsync(command);
            var previewKey = command.GetPreviewkey();
            var previewReportFileDto = new PreviewReportFileDto(command.ActivityId, previewKey, $"rpt/workers/preview/url");
            return Ok(previewReportFileDto);
        }

        [HttpPost("preview/url")]
        public IActionResult GetReportFileUrl([FromBody] GetReportFileUrl command)
        {
            var fileUrl = _memoryCache.Get<string>(command.PreviewKey);
            var previewDto = new PreviewReportDto(fileUrl);
            return Ok(previewDto);
        }
    }
}