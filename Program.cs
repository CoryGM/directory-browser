using Browser.Directories.Browse;
using Browser.Directories.Browse.Service;
using Browser.Files.Download.Service;
using Browser.Files.Upload.Service;

namespace Browser {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            var browseConfig = new BrowseConfiguration();
            builder.Configuration.Bind("Browse", browseConfig);
            builder.Services.AddSingleton(browseConfig);

            // Add services to the container.
            builder.Services.AddTransient<IDirectoryBrowserService, DirectoryBrowserService>();
            builder.Services.AddTransient<IFileDownloadService, FileDownloadService>();
            builder.Services.AddTransient<IFileUploadService, FileUploadService>();

            builder.Services.AddProblemDetails();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DefaultModelsExpandDepth(-1);
                });
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}