using System.Collections.Generic;
using Stl2Img.Models;

namespace Stl2Img.Helpers.Comparers {
    public class RenderedStlfInfoComparer :IEqualityComparer<RenderedSTLInfo> {
        public bool Equals(RenderedSTLInfo x, RenderedSTLInfo y) {
            if (x == null) {
                return y == null;
            }
            
            if (y == null) {
                return false;
            }
            return x.StlPath == y.StlPath;
        }

        public int GetHashCode(RenderedSTLInfo obj) {
            return obj.StlPath.GetHashCode();
        }
    }
}