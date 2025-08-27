using System.Reflection;

namespace Browser.Directories.Browse.Service
{
    public class DirectoryBrowserService : IDirectoryBrowserService
    {
        private readonly ILogger Logger;
        private readonly string ClassName;

        public DirectoryBrowserService(ILogger<DirectoryBrowserService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Logger = logger;
            ClassName = GetType().Name;
        }

        /// <summary>
        /// Gets the contents of a directory asynchronously.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DirectoryBrowserServiceResult> GetContentsAsync(DirectoryBrowserOptions options,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetContents(options), cancellationToken);
        }

        /// <summary>
        /// Gets the contents of a directory. This is a synchronous method.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DirectoryBrowserServiceResult GetContents(DirectoryBrowserOptions options)
        {
            var result = new DirectoryBrowserServiceResult()
            {
                BasePath = options.BasePath,
                SearchPattern = options.SearchPattern,
                IncludeSubdirectories = options.IncludeSubdirectories
            };

            var fileSearchPattern = options.SearchPattern ?? "*.*";
            var directorySearchPattern = GetDirectorySearchPattern(fileSearchPattern);
            var searchOption = options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (string.IsNullOrWhiteSpace(options.BasePath))
            {
                result.SetBadRequest("Path is null or empty.");

                return result;
            }

            if (!Directory.Exists(options.BasePath))
            {
                result.IsSuccess = false;
                result.AddMessage("Directory does not exist.");

                return result;
            }

            try
            {
                result.MatchedFiles = Directory.GetFiles(options.BasePath,
                    fileSearchPattern, searchOption);

                if (!String.IsNullOrWhiteSpace(directorySearchPattern))
                    result.MatchedDirectories = Directory.GetDirectories(options.BasePath,
                        directorySearchPattern, searchOption);
            }
            catch (Exception ex)
            {
                var msg = $"Error retrieving files";
                Logger.LogError(ex, $"{ClassName}.{MethodBase.GetCurrentMethod()}: {msg}");
                result.SetException($"{msg}: Check the log for details.");

                return result;
            }

            return result;
        }

        private string GetDirectorySearchPattern(string fileSearchPattern)
        {
            if (string.IsNullOrWhiteSpace(fileSearchPattern) ||
                fileSearchPattern.Equals("*.*"))
                return "*";

            if (fileSearchPattern.Contains('.'))
                return String.Empty;

            return fileSearchPattern;
        }
    }
}
