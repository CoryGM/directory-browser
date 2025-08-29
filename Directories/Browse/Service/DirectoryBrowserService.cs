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
                CurrentPath = String.IsNullOrWhiteSpace(options.CurrentPath) ?
                    $"{Path.DirectorySeparatorChar}" : 
                    options.CurrentPath,
                SearchPattern = options.SearchTerm,
                IncludeSubdirectories = options.IncludeSubdirectories
            };

            if (string.IsNullOrWhiteSpace(BrowseConfig.HomeDirectory))
            {
                result.SetBadRequest("Home Directory is null or empty.");

                return result;
            }

            var fileSearchPattern = options.SearchTerm ?? "*.*";
            var directorySearchPattern = GetDirectorySearchPattern(fileSearchPattern);
            var searchOption = options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var fullPath = String.IsNullOrWhiteSpace(options.CurrentPath) ?
                BrowseConfig.HomeDirectory :
                Path.Combine(BrowseConfig.HomeDirectory, options.CurrentPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (!Directory.Exists(fullPath))
            {
                result.IsSuccess = false;
                result.AddMessage("Directory does not exist.");

                return result;
            }

            try
            {
                result.MatchedFiles = GetMatchedFiles(fullPath, fileSearchPattern, searchOption);

                if (!String.IsNullOrWhiteSpace(directorySearchPattern))
                    result.MatchedDirectories = GetMatchedDirectories(fullPath, directorySearchPattern, searchOption);
            }
            catch (Exception ex)
            {
                var msg = $"Error retrieving files";
                Logger.LogError(ex, $"{ClassName}.{MethodBase.GetCurrentMethod()}: {msg}");
                result.SetException(msg);

                return result;
            }

            return result;
        }

        private static List<MatchedDirectory> GetMatchedDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            var dirInfo = new DirectoryInfo(path);

            return [.. dirInfo.GetDirectories(searchPattern, searchOption).Select(d => GetMatchedDirectory(path, d))];
        }

        private static MatchedDirectory GetMatchedDirectory(string path, DirectoryInfo dirInfo)
        {
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            return new MatchedDirectory
            {
                Name = String.IsNullOrWhiteSpace(path) ?
                        dirInfo.FullName :
                        dirInfo.FullName.Replace(path, String.Empty),
                SizeInBytes = files.Sum(f => f.Length),
                FileCount = files.Length,
            };
        }

        private static List<MatchedFile> GetMatchedFiles(string path, string searchPattern, SearchOption searchOption)
        {
            var dirInfo = new DirectoryInfo(path);

            return [.. dirInfo.GetFiles(searchPattern, searchOption)
                .Select(f => new MatchedFile
                {
                    PathName = String.IsNullOrWhiteSpace(path) ? 
                        f.FullName : 
                        f.FullName.Replace(path, String.Empty),
                    SizeInBytes = f.Length
                })];
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
