using System.Collections.Generic;
using LiteDB;
using Stl2Img.Models;

namespace Stl2Img.Repository {
    public interface IStlRepository {
        List<RenderedSTLInfo> GetAllStlInfos();
        void InsertInfo(List<RenderedSTLInfo> inserts);
        void InsertInfo(RenderedSTLInfo insert);
        RenderedSTLInfo GetById(ObjectId id);
        RenderedSTLInfo GetByStlFilePath(string path);
    }
}