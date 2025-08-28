namespace Browser.Directories.Browse.Api
{
    public class MatchedDirectory
    {
        public string? Name { get; init; }
        public long SizeInBytes { get; init; } = 0;
        public long FileCount { get; init; } = 0;
    }
}
