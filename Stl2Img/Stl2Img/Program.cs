using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Stl2Img.Models;
using Stl2Img.Processors.Rendering;
using Stl2Img.Repository;

namespace Stl2Img {
    class Program {
        //static void Main(string[] args) {
        //    AsyncContext.Run(AsyncMain);
        //}

        public static async Task Main(string[] args) {
            Initialize();
            Log.Information($"Starting STL2IMG");

            var config = GetConfiguration();
            var files = GetStlFilesInPath(config.FolderToSearch);

            IRenderImageProcessor renderer = RenderFactory.Create(config, new LiteDbRepository(Path.Combine(Directory.GetCurrentDirectory(),"stl2imgDb.db")));
            await renderer.RenderStlFilesAsync(files);
            Log.CloseAndFlush();
        }

        private static void Initialize() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        private static Stl2ImgConfiguration GetConfiguration() {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Information($"Getting configuration");
            Stl2ImgConfiguration config = configurationRoot.Get<Stl2ImgConfiguration>();
            return config;
        }

        private static List<string> GetStlFilesInPath(string path) {
            Log.Information($"Getting all files in {path}");
            return Directory.GetFiles(path, "*.stl", SearchOption.AllDirectories).ToList();
        }
    }
}