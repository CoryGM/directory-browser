using Browser.Core;

namespace Browser.Directories.Browse.Service
{
    public class DirectoryBrowserServiceResult : OperationResult
    {
        private readonly List<MatchedFile> _matchedFiles = [];
        private readonly List<MatchedDirectory> _matchedDirectories = [];

        public string? BasePath { get; set; }
        public string? SearchPattern { get; set; }
        public bool IncludeSubdirectories { get; set; }

        public IEnumerable<MatchedFile> MatchedFiles 
        { 
            get => [.. _matchedFiles]; 
            set
            {
                _matchedFiles.Clear();

                if (value is not null)
                    _matchedFiles.AddRange(value);
            } 
        }

        public IEnumerable<MatchedDirectory> MatchedDirectories
        { 
            get => [.. _matchedDirectories]; 
            set
            {
                _matchedDirectories.Clear();

                if (value is not null)
                    _matchedDirectories.AddRange(value);
            }
        }
    }
}
