
namespace Browser.Files.Upload.Service
{
    public interface IFileUploadService
    {
        Task<FileUploadServiceResult> UploadAsync(FileUploadOptions options, CancellationToken cancellationToken = default);
    }
}