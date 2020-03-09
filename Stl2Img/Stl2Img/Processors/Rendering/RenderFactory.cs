using Serilog;
using Stl2Img.Models;
using Stl2Img.Repository;

namespace Stl2Img.Processors.Rendering {
    public class RenderFactory {
        public static IRenderImageProcessor Create(Stl2ImgConfiguration config, IStlRepository repository) {
            Log.Information($"Creating Renderer");
            return new OpenSCADRenderer(config, repository);
        }
    }
}