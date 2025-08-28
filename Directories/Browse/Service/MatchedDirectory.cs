namespace Browser.Directories.Browse.Service
{
    public class MatchedDirectory
    {
        public string? Name { get; set; }
        public long SizeInBytes { get; set; } = 0;
        public long FileCount { get; set; } = 0;
    }
}
