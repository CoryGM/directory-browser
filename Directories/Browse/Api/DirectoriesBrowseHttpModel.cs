using Browser.Directories.Browse.Service;

namespace Browser.Directories.Browse.Api
{
    public class DirectoriesBrowseHttpModel
    {
        public DirectoriesBrowseHttpModel(DirectoryBrowserServiceResult serviceResult) 
        { 
            CurrentPath = serviceResult.CurrentPath;
            SearchTerm = serviceResult.SearchPattern;
            IncludeSubdirectories = serviceResult.IncludeSubdirectories;
            MatchedFileDirectories = GetMatchedFileDirectories(serviceResult);
            MatchedDirectories = GetMatchedDirectories(serviceResult);
        }

        public string? CurrentPath { get; set; }
        public string? SearchTerm { get; set; }
        public bool IncludeSubdirectories { get; set; }

        private readonly List<MatchedFileDirectory> _matchedFileDirectories = [];
        public IEnumerable<MatchedFileDirectory> MatchedFileDirectories 
        { 
            get => [.. _matchedFileDirectories]; 
            set
            {
                _matchedFileDirectories.Clear();

                if (value is not null)
                    _matchedFileDirectories.AddRange(value);
            } 
        }

        private readonly List<MatchedDirectory> _matchedDirectories = [];
        public IEnumerable<MatchedDirectory> MatchedDirectories
        { 
            get => [.._matchedDirectories]; 
            set
            {
                _matchedDirectories.Clear();

                if (value is not null)
                    _matchedDirectories.AddRange(value);
            }
        }

        private static List<MatchedDirectory> GetMatchedDirectories(DirectoryBrowserServiceResult serviceResult)
        {
            if (serviceResult?.MatchedDirectories is null || !serviceResult.MatchedDirectories.Any())
                return [];

            return [.. serviceResult.MatchedDirectories.Select(md => new MatchedDirectory()
            {
                Name = md.Name,
                SizeInBytes = md.SizeInBytes,
                FileCount = md.FileCount
            })];
        }

        private static List<MatchedFileDirectory> GetMatchedFileDirectories(DirectoryBrowserServiceResult serviceResult)
        {
            if (serviceResult?.MatchedFiles is null || !serviceResult.MatchedFiles.Any())
                return [];

            var groupedFiles = serviceResult.MatchedFiles
                .Select(mf => new
                {
                    DirectoryName = Path.GetDirectoryName(mf.PathName),
                    FileName = Path.GetFileName(mf.PathName),
                    mf.SizeInBytes
                })
                .GroupBy(mf => mf.DirectoryName ?? string.Empty)
                .Select(g => new MatchedFileDirectory()
                { 
                    Name = g.Key,
                    FileCount = g.Count(),
                    Size = GetFileDisplaySize(g.Sum(f => f.SizeInBytes)),
                    Files = [.. g.Select(f => new MatchedFile(f.FileName, GetFileDisplaySize(f.SizeInBytes)))]
                });

            return [.. groupedFiles];
        }

        private static string? GetFileDisplaySize(long? fileSize)
        {
            return fileSize switch
            {
                null => null,
                < 0 => "Invalid size",
                0 => "0 B",
                < 1024 => $"{fileSize} B",
                < 1048576 => $"{fileSize / 1024.0:F2} KB",
                < 1073741824 => $"{fileSize / 1048576.0:F2} MB",
                _ => $"{fileSize / 1073741824.0:F2} GB"
            };
    }
    }
}
