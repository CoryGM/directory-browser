using Microsoft.AspNetCore.Mvc;
using Browser.Directories.Browse.Service;
using Browser.Directories.Browse.Api;

namespace Browser.Controllers
{
    [Route("api/directories")]
    [ApiController]
    public class DirectoriesController : ControllerBase
    {
        private readonly ILogger Logger;
        private readonly string ControllerName;

        public DirectoriesController(ILogger<DirectoriesController> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Logger = logger;
            ControllerName = GetType().Name;
        }

        [HttpGet]
        [Route("browse")]
        public async Task<ActionResult<DirectoriesBrowseHttpModel>> BrowseAsync([FromServices] IDirectoryBrowserService browser,
            [FromQuery] Directories.Browse.Service.DirectoryBrowserOptions? options,
            CancellationToken cancellationToken)
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            if (options is null)
            {
                //  want to seed the logs with a message containing the
                //  controller name and action name
                var msg = "Options are required.";
                Logger.LogWarning($"{ControllerName}.{actionName}: {msg}");

                return Problem(detail: msg, statusCode: 400);
            }

            var result = await browser.GetContentsAsync(options, cancellationToken);

            if (result.IsBadRequest)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 400);

            if (result.IsException)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 500);

            return Ok(new DirectoriesBrowseHttpModel(result));
        }
    }
}
