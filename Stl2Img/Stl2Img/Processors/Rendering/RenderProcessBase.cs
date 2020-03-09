using System.Collections.Generic;
using System.Threading.Tasks;
using Stl2Img.Models;
using Stl2Img.Repository;

namespace Stl2Img.Processors.Rendering {
    public abstract class RenderProcessBase : IRenderImageProcessor {
        protected readonly Stl2ImgConfiguration Configuration;
        protected readonly IStlRepository Repository;

        public RenderProcessBase(Stl2ImgConfiguration configuration, IStlRepository repository) {
            Configuration = configuration;
            Repository = repository;
        }
        public abstract Task<List<RenderedSTLInfo>> RenderStlFilesAsync(List<string> files);
    }
}