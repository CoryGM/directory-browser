using System.Reflection;

using Browser.Directories.Browse;

namespace Browser.Files.Download.Service
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly ILogger Logger;
        private readonly string ClassName;
        private readonly BrowseConfiguration BrowseConfig;

        public FileDownloadService(BrowseConfiguration browseConfig,
            ILogger<FileDownloadService> logger)
        {
            ArgumentNullException.ThrowIfNull(browseConfig);
            ArgumentNullException.ThrowIfNull(logger);

            BrowseConfig = browseConfig;
            Logger = logger;
            ClassName = GetType().Name;
        }

        /// <summary>
        /// Gets the contents of a file asynchronously in a byte array.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileDownloadServiceResult> GetBytesAsync(FileDownloadOptions options,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetBytes(options), cancellationToken);
        }


        /// <summary>
        /// Gets the contents of a file in a byte array. This is a synchronous method.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public FileDownloadServiceResult GetBytes(FileDownloadOptions options)
        {
            var result = new FileDownloadServiceResult();

            if (string.IsNullOrWhiteSpace(BrowseConfig.HomeDirectory))
            {
                result.SetBadRequest("Home Directory is null or empty.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(options.FilePath))
            {
                result.SetBadRequest("FilePath is null or empty.");
                return result;
            }

            var fullPath = Path.GetFullPath(options.FilePath);

            if (!fullPath.StartsWith(BrowseConfig.HomeDirectory, StringComparison.OrdinalIgnoreCase))
                fullPath = Path.Combine(BrowseConfig.HomeDirectory, fullPath.TrimStart(Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
            {
                result.SetBadRequest("The specified file does not exist.");
                return result;
            }

            try
            {
                var fileBytes = File.ReadAllBytes(fullPath);

                result.FileBytes = fileBytes;
                result.FileName = Path.GetFileName(fullPath);
                result.FilePath = Path.GetDirectoryName(fullPath);
                result.ContentType = "application/octet-stream";
            }
            catch (Exception ex)
            {
                string msg = "An error occurred while reading the file.";
                Logger.LogError(ex, $"{ClassName}.{MethodBase.GetCurrentMethod()}: Full Path: {fullPath}, Message: {msg}");
                result.SetException("An error occurred while reading the file. Check the log for details.");
            }

            return result;
        }
    }
}
