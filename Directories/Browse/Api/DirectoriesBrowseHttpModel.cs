using Browser.Directories.Browse.Service;

namespace Browser.Directories.Browse.Api
{
    public record struct DirectoriesBrowseHttpModel
    {
        public DirectoriesBrowseHttpModel(DirectoryBrowserServiceResult serviceResult) 
        { 
            BasePath = serviceResult.BasePath;
            SearchPattern = serviceResult.SearchPattern;
            IncludeSubdirectories = serviceResult.IncludeSubdirectories;
            MatchedFiles = [.. serviceResult.MatchedFiles];
            MatchedDirectories = [.. serviceResult.MatchedDirectories];
        }

        public string? BasePath { get; set; }
        public string? SearchPattern { get; set; }
        public bool IncludeSubdirectories { get; set; }

        public string[] MatchedFiles { get; init; }
        public string[] MatchedDirectories { get; init; }
    }
}
