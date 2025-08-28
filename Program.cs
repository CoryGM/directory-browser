using Browser.Directories.Browse;
using Browser.Directories.Browse.Service;

namespace Browser {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            var browseConfig = new BrowseConfiguration();
            builder.Configuration.Bind("Browse", browseConfig);
            builder.Services.AddSingleton(browseConfig);

            // Add services to the container.
            builder.Services.AddTransient<IDirectoryBrowserService, DirectoryBrowserService>();

            builder.Services.AddProblemDetails();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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