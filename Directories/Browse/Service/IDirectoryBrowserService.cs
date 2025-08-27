namespace Browser.Directories.Browse.Service
{
    public interface IDirectoryBrowserService
    {
        Task<DirectoryBrowserServiceResult> GetContentsAsync(DirectoryBrowserOptions options, CancellationToken cancellationToken = default);
        DirectoryBrowserServiceResult GetContents(DirectoryBrowserOptions options);
    }
}