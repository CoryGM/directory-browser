namespace Browser.Files.Upload.Service
{
    public class FileUploadOptions
    {
        public IFormFile? File { get; set; }
        public string? TargetPath { get; set; }
    }
}
