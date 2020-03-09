using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Stl2Img.Helpers;
using Stl2Img.Helpers.Comparers;
using Stl2Img.Models;
using Stl2Img.Repository;
using ToolBox.Bridge;
using ToolBox.Notification;
using ToolBox.Platform;

namespace Stl2Img.Processors.Rendering {
    public class OpenSCADRenderer : RenderProcessBase, IDisposable {
        private readonly string _tempFileName;
        private string _outputPath;
        private readonly List<RenderedSTLInfo> _dbRenderedInfo;
        private readonly Stopwatch _timer;
    
        protected string OutputPath {
            get { return _outputPath ??= GetOutputPath(); }
        }

        private string GetOutputPath() {
            string path = Path.Combine(Configuration.CatalogFolder, Configuration.ImageFolderName);
            Directory.CreateDirectory(path);
            return path;
        }

        public OpenSCADRenderer(Stl2ImgConfiguration configuration, IStlRepository repository) : base(configuration, repository) {
            Log.Information($"OpenSCAD rendered created.");
            _tempFileName = Configuration.FolderToSearch + $"\\tempSCAD.scad";
            CreateTempSCADFile();
            _dbRenderedInfo = Repository.GetAllStlInfos();
            _timer = new Stopwatch();
            _timer.Start();
            //Styles = new List<string>(){"Cornfield", "Metallic", "Sunset", "Starnight", "BeforeDawn", "Nature", "DeepOcean", "Solarized", "Tomorrow", "Tomorrow 2", "Tomorrow Night", "Monotone"};
        }

        public async override Task<List<RenderedSTLInfo>> RenderStlFilesAsync(List<string> files) {

            if (Configuration.MultiThreading) {
                ConcurrentBag<RenderedSTLInfo> resultCollection = new ConcurrentBag<RenderedSTLInfo>();

                ParallelLoopResult loopResult = Parallel.ForEach(files,
                    new ParallelOptions {MaxDegreeOfParallelism = Configuration.MaximumConcurrentThreads},
                    stlFile => {
                        RenderStlFile(stlFile);
                    });

                
            }
            else {
                foreach (string stlFile in files) {
                    RenderStlFile(stlFile);
                }
            }

            var result = Repository.GetAllStlInfos().Except(_dbRenderedInfo, new RenderedStlfInfoComparer()).ToList();

            _timer.Stop();
            TimeSpan time = TimeSpan.FromTicks(_timer.ElapsedTicks);
            
            TimeSpan avgTime = TimeSpan.Zero;
            if (result.Count > 0) {
                avgTime = TimeSpan.FromTicks(_timer.ElapsedTicks / result.Count);
            }

            Log.Information($"Complete set of {files.Count} STL files took {time.Hours}h {time.Minutes}m {time.Seconds}s {time.Milliseconds}ms.");
            Log.Information($"one STL file took on average {avgTime.Minutes}m {avgTime.Seconds}s {avgTime.Milliseconds}ms.");
            Log.Information($"In total there were {result.Count} images outputted.");
            return result;
        }

        private RenderedSTLInfo CheckDatabaseRecordsForFile(string file) {
            try {
                return _dbRenderedInfo.SingleOrDefault(x => x.StlPath == file);
            }catch (Exception e) {
                string message = $"Too many records for the same file: {file}. {e.Message}";
                Log.Error(message);
            }

            return null;
        }

        private RenderedSTLInfo RenderStlFile(string file) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            RenderedSTLInfo stlInfo = CheckDatabaseRecordsForFile(file);
            if (stlInfo == null) {
                // check time blabla
                string outputFile = GetOutputFileName(file);
                stlInfo = SendCommand(file, outputFile, Configuration.OpenSCADLocation, Configuration.ColorScheme, Configuration.OutputImageWidth, Configuration.OutputImageHeight);
                Repository.InsertInfo(stlInfo);
            }

            stopwatch.Stop();
            Log.Information($"{Path.GetFileNameWithoutExtension(file)}: {stopwatch.ElapsedMilliseconds}ms");

            return stlInfo;
        }

        private string GetOutputFileName(string file) {
            string filename = Path.GetFileNameWithoutExtension(file)  + ".png";
            return Path.Combine(OutputPath, filename);
        }

        private ShellConfigurator GetShellConfigurator() {
            var notificationSystem = NotificationSystem.Default;
            IBridgeSystem bridgeSystem = null;
            switch (OS.GetCurrent()) {
                case "win":
                    bridgeSystem = BridgeSystem.Bat;
                    break;
                case "mac":
                case "gnu":
                    bridgeSystem = BridgeSystem.Bash;
                    break;
            }

            return new ShellConfigurator(bridgeSystem, notificationSystem);
        }

        private RenderedSTLInfo SendCommand(string inputFile, string outputFile, string openSCADLocation, string colorScheme = "Cornfield", int width = 720, int height = 1024) {
            var oriShell = GetShellConfigurator();

            var relPath = inputFile.Substring(Configuration.FolderToSearch.Length+1).Replace('\\','/');

            var command = $"\"{openSCADLocation.EscapeStringForCommand()}\" -D \"model=\"\"\"{relPath.EscapeStringForCommand()}\"\" -o \"{outputFile.EscapeStringForCommand()}\" --autocenter --viewall --imgsize=\"{width},{height}\" --colorscheme=\"{colorScheme}\" \"{_tempFileName}\"";
            oriShell.Term(command);
            return new RenderedSTLInfo(inputFile, outputFile);
        }

        private void CreateTempSCADFile() {
            var scadContents = $"model = \"\";{Environment.NewLine}import(file=model);";
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(_tempFileName)) {
                File.Delete(_tempFileName);
            }

            // Create a new file     
            File.WriteAllText(_tempFileName, scadContents);
        }

        private void ReleaseUnmanagedResources() {
            if (File.Exists(_tempFileName)) {
                File.Delete(_tempFileName);
            }
        }

        public void Dispose() {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~OpenSCADRenderer() {
            ReleaseUnmanagedResources();
        }
    }
}