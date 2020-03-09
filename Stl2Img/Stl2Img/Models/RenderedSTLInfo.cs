using System;

namespace Stl2Img.Models {
    public class RenderedSTLInfo : LiteDbBasePoco {

        public RenderedSTLInfo(string stlFilePath, string imgFilePath) {
            RenderedAtDateTime = DateTime.Now;
            StlPath = stlFilePath;
            ImgPath = imgFilePath;
        }
        public string StlPath { get; set; }
        public string ImgPath { get; set; }
        public DateTime RenderedAtDateTime{ get; set; }
    }
}