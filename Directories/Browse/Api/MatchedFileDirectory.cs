namespace Browser.Directories.Browse.Api
{
    public class MatchedFileDirectory
    {
        public string? Name { get; init; }
        public long SizeInBytes { get; init; } = 0;
        public long FileCount { get; init; } = 0;

        private readonly List<MatchedFile> _files = [];
        public IEnumerable<MatchedFile> Files 
        { 
            get => [.. _files]; 
            init
            {
                _files.Clear();

                if (value is not null)
                    _files.AddRange(value);
            } 
        }
    }
}
