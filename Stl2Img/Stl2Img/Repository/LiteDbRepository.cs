using System.Collections.Generic;
using LiteDB;
using Stl2Img.Models;

namespace Stl2Img.Repository {
    public class LiteDbRepository : IStlRepository {
        private readonly object _dbLock = new object();
        protected LiteDatabase LiteDatabase { get; set; }
        protected string FileName { get; set; }
        public LiteDbRepository(string fileName) {
            FileName = fileName;
        }

        protected const string RenderedStlInfoCollectionName = "renderedStlInfo";

        public List<RenderedSTLInfo> GetAllStlInfos() {
            lock (_dbLock) {
                using (LiteDatabase = new LiteDatabase(FileName)) {
                    var collection = LiteDatabase.GetCollection<RenderedSTLInfo>(RenderedStlInfoCollectionName);
                    return collection.Query().ToList();
                }
            }
        }

        public void InsertInfo(List<RenderedSTLInfo> inserts) {
            lock (_dbLock) {
                using (LiteDatabase = new LiteDatabase(FileName)) {
                    var collection = LiteDatabase.GetCollection<RenderedSTLInfo>(RenderedStlInfoCollectionName);
                    collection.Insert(inserts);
                }
            }
        }

        public void InsertInfo(RenderedSTLInfo insert) {
            lock (_dbLock) {
                using (LiteDatabase = new LiteDatabase(FileName)) {
                    var collection = LiteDatabase.GetCollection<RenderedSTLInfo>(RenderedStlInfoCollectionName);
                    collection.Insert(insert);
                }
            }
        }

        public RenderedSTLInfo GetById(ObjectId id) {
            lock (_dbLock) {
                using (LiteDatabase = new LiteDatabase(FileName)) {
                    var collection = LiteDatabase.GetCollection<RenderedSTLInfo>(RenderedStlInfoCollectionName);
                    return collection.Query().Where(x=>x._id == id).Limit(1).SingleOrDefault();
                }
            }
        }

        public RenderedSTLInfo GetByStlFilePath (string path) {
            lock (_dbLock) {
                using (LiteDatabase = new LiteDatabase(FileName)) {
                    var collection = LiteDatabase.GetCollection<RenderedSTLInfo>(RenderedStlInfoCollectionName);
                    return collection.Query().Where(x=>x.StlPath == path).Limit(1).SingleOrDefault();
                }
            }
        }
    }
}