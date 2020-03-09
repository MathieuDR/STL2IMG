namespace Stl2Img.Models {
    public class Stl2ImgConfiguration {
        public string FolderToSearch { get; set; }
        public string CatalogFolder { get; set; }
        public string ImageFolderName { get; set; }
        public string OpenSCADLocation { get; set; }
        public int MaximumConcurrentThreads { get; set; }
        public bool MultiThreading { get; set; }
        public string ColorScheme { get; set; }
        public int OutputImageWidth { get; set; }
        public int OutputImageHeight { get; set; }
    }
}