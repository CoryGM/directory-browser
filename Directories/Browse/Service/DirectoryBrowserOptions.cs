namespace Browser.Directories.Browse.Service
{
    public class DirectoryBrowserOptions
    {
        public string? BasePath { get; init; }
        public string? SearchPattern { get; init; }
        public bool IncludeSubdirectories { get; init; }
    }
}
