using Browser.Directories.Browse;
using System.Reflection;

namespace Browser.Files.Upload.Service
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ILogger Logger;
        private readonly string ClassName;
        private readonly BrowseConfiguration BrowseConfig;
        private readonly string[] ForbiddenExtensions = {
            ".action", ".apk", ".app", ".bat", ".bin", ".cab", ".cmd", ".com", ".command",
            ".cpl", ".csh", ".exe", ".gadget", ".inf1", ".ins", ".inx", ".ipa", ".isu",
            ".job", ".jse", ".ksh", ".lnk", ".msc", ".msi", ".msp", ".mst", ".osx", ".out",
            ".paf", ".pif", ".prg", ".ps1", ".reg", ".rgs", ".run", ".scr", ".sct", ".shb",
            ".shs", ".u3p", ".vb", ".vbe", ".vbs", ".vbscript", ".workflow", ".ws", ".wsf", ".wsh" };

        public FileUploadService(BrowseConfiguration browseConfig,
            ILogger<FileUploadService> logger)
        {
            ArgumentNullException.ThrowIfNull(browseConfig);
            ArgumentNullException.ThrowIfNull(logger);

            BrowseConfig = browseConfig;
            Logger = logger;
            ClassName = GetType().Name;
        }

        public async Task<FileUploadServiceResult> UploadAsync(FileUploadOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = new FileUploadServiceResult();

            if (options.File is null || options.File.Length == 0)
            { 
                result.SetBadRequest("No file uploaded.");
                return result;
            }

            if (String.IsNullOrWhiteSpace(options.TargetPath))
            {
                result.SetBadRequest("A target destination for the update is required.");
                return result;
            }

            var fileExtension = Path.GetExtension(options.File.FileName).ToLowerInvariant();

            if (ForbiddenExtensions.Contains(fileExtension))
            {
                result.SetBadRequest($"Files with extension '{fileExtension}' are not allowed.");
                return result;
            }

            var basePath = BrowseConfig.HomeDirectory ?? "";
            var safeTargetPath = options.TargetPath?.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) ?? "";
            var uploadPath = Path.Combine(basePath, safeTargetPath);

            if (!Directory.Exists(uploadPath))
            {
                result.SetBadRequest($"'{safeTargetPath}' does not exist.");
                return result;
            }

            var filePath = Path.Combine(uploadPath, Path.GetFileName(options.File.FileName));

            if (File.Exists(filePath))
            {
                result.SetBadRequest($"'{options.File.FileName}' already exists at the target destination.");
                return result;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    await options.File.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
                    result.AddMessage($"'{options.File.FileName}' uploaded successfully.");
                }
                catch (Exception ex)
                {
                    var msg = "There was an error uploading the file.";
                    Logger.LogError(ex, $"{ClassName}.{MethodBase.GetCurrentMethod()}: Message: {msg}");
                    result.SetException(msg);
                }
            }

            return result;
        }
    }
}
