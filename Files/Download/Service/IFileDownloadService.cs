
namespace Browser.Files.Download.Service
{
    public interface IFileDownloadService
    {
        FileDownloadServiceResult GetBytes(FileDownloadOptions options);
        Task<FileDownloadServiceResult> GetBytesAsync(FileDownloadOptions options, CancellationToken cancellationToken = default);
    }
}