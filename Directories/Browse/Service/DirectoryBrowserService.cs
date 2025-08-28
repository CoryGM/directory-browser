using System.Reflection;

namespace Browser.Directories.Browse.Service
{
    public class DirectoryBrowserService : IDirectoryBrowserService
    {
        private readonly ILogger Logger;
        private readonly string ClassName;
        private readonly BrowseConfiguration BrowseConfig;

        public DirectoryBrowserService(BrowseConfiguration browseConfig,
            ILogger<DirectoryBrowserService> logger)
        {
            ArgumentNullException.ThrowIfNull(browseConfig);
            ArgumentNullException.ThrowIfNull(logger);

            BrowseConfig = browseConfig;
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
                BasePath = BrowseConfig.HomeDirectory,
                SearchPattern = options.SearchTerm,
                IncludeSubdirectories = options.IncludeSubdirectories
            };

            var fileSearchPattern = options.SearchTerm ?? "*.*";
            var directorySearchPattern = GetDirectorySearchPattern(fileSearchPattern);
            var searchOption = options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (string.IsNullOrWhiteSpace(BrowseConfig.HomeDirectory))
            {
                result.SetBadRequest("Home Directory is null or empty.");

                return result;
            }

            if (!Directory.Exists(BrowseConfig.HomeDirectory))
            {
                result.IsSuccess = false;
                result.AddMessage("Directory does not exist.");

                return result;
            }

            try
            {
                result.MatchedFiles = Directory.GetFiles(BrowseConfig.HomeDirectory,
                    fileSearchPattern, searchOption).Select(x => x.Replace(BrowseConfig.HomeDirectory, String.Empty));

                if (!String.IsNullOrWhiteSpace(directorySearchPattern))
                    result.MatchedDirectories = Directory.GetDirectories(BrowseConfig.HomeDirectory,
                        directorySearchPattern, searchOption).Select(x => x.Replace(BrowseConfig.HomeDirectory, String.Empty));
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
