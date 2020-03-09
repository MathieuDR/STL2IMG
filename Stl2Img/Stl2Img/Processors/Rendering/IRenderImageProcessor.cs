using System.Collections.Generic;
using System.Threading.Tasks;
using Stl2Img.Models;

namespace Stl2Img.Processors.Rendering {
    public interface IRenderImageProcessor {
        Task<List<RenderedSTLInfo>> RenderStlFilesAsync(List<string> files);
    }
}