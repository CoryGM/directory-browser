namespace Browser.Directories.Browse.Service
{
    public class DirectoryBrowserOptions
    {
        public string? CurrentPath { get; init; }
        public string? SearchTerm { get; init; }
        public bool IncludeSubdirectories { get; init; }
    }
}
