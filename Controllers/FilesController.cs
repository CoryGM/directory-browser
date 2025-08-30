using Microsoft.AspNetCore.Mvc;

using Browser.Files.Download.Service;
using Browser.Files.Upload.Service;

namespace Browser.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ILogger Logger;
        private readonly string ControllerName;

        public FilesController(ILogger<FilesController> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Logger = logger;
            ControllerName = GetType().Name;
        }

        [HttpGet]
        [Route("download")]
        public async Task<ActionResult<byte[]>> DownloadAsync([FromServices] IFileDownloadService download,
            [FromQuery] FileDownloadOptions? options,
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

            var result = await download.GetBytesAsync(options, cancellationToken);

            if (result.IsBadRequest)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 400);

            if (result.IsException)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 500);

            return File(result.FileBytes!, result.ContentType!, result.FileName);
        }

        [HttpPost]
        [Route("upload")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadAsync([FromServices] IFileUploadService upload,
            [FromForm] IFormFile file,
            [FromForm] string targetPath,
            CancellationToken cancellationToken)
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            var options = new FileUploadOptions()
            {
                File = file,
                TargetPath = targetPath
            };

            var result = await upload.UploadAsync(options, cancellationToken);

            if (result.IsBadRequest)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 400);

            if (result.IsException)
                return Problem(detail: string.Join(Environment.NewLine, result.Messages),
                    statusCode: 500);

            return Ok(new { message = "File uploaded successfully." });
        }
    }
}
