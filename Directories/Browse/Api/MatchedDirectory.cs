namespace Browser.Directories.Browse.Api
{
    public class MatchedDirectory
    {
        public string? Name { get; init; }
        public string Size { get; init; }
        public long FileCount { get; init; } = 0;
    }
}
