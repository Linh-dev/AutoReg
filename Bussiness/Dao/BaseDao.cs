using Bussiness.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bussiness.Dao
{
    public class BaseDao<T> where T : BaseModelInfo
    {
        private string collectionName = "";
        public BaseDao() { }
        public BaseDao(string collection)
        {
            collectionName = collection;
        }

        public IMongoCollection<T> GetCollection()
        {
            var db = Database.GetDatabase();
            var collection = db.GetCollection<T>(collectionName);
            return collection;
        }

        public List<T> GetAll()
        {
            var collection = GetCollection();
            return collection.AsQueryable().ToList();
        }

        public List<T> GetAllActive()
        {
            var collection = GetCollection();
            var filter = Builders<T>.Filter.Empty;
            filter &= Builders<T>.Filter.Eq("IsDeleted", false);
            return collection.Find(filter).ToList();
        }
        public T GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default(T);
            return GetById(new ObjectId(id));
        }

        public T GetById(ObjectId id)
        {
            var collection = GetCollection();
            var filtter = Builders<T>.Filter.Eq("_id", id);
            return collection.Find(filtter).FirstOrDefault();
        }
        public List<T> GetByListId(IEnumerable<ObjectId> listId)
        {
            if (listId == null) return null;
            var collection = GetCollection();
            var filtter = Builders<T>.Filter.In("_id", listId);
            return collection.Find(filtter).ToList();
        }

        public void Insert(T info)
        {
            var collection = GetCollection();
            collection.InsertOne(info);
        }

        public void InsertRange(List<T> info)
        {
            var collection = GetCollection();
            collection.InsertMany(info);

        }

        public void Replace(T info)
        {
            var collection = GetCollection();
            var filtter = Builders<T>.Filter.Eq("_id", info._id);
            collection.ReplaceOne(filtter, info);
        }
        public void Delete(ObjectId _id)
        {
            var collection = GetCollection();
            var filter = Builders<T>.Filter.Eq("_id", _id);
            var rs = collection.DeleteOne(filter);
        }
        public void Delete(string _id)
        {
            Delete(new ObjectId(_id));
        }

        public void DeleteRange(IEnumerable<ObjectId> idList)
        {
            var collection = GetCollection();
            var filter = Builders<T>.Filter.In("_id", idList);
            var rs = collection.DeleteMany(filter);
        }

        public int CountAll()
        {
            var collection = GetCollection();
            return collection.AsQueryable().Count();
        }
        public long CountAllActive(bool IsDeleted = false)
        {
            var collection = GetCollection();
            var filtter = Builders<T>.Filter.Eq("IsDeleted", IsDeleted);
            return collection.Find(filtter).Count();
        }
    }
}
