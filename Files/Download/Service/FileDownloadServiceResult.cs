using Browser.Core;

namespace Browser.Files.Download.Service
{
    public class FileDownloadServiceResult : OperationResult
    {
        public byte[]? FileBytes { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }
}
