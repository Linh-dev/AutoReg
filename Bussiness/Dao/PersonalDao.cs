using Bussiness.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bussiness.Dao
{
    public class PersonalDao : BaseDao<PersonalInfo>
    {
        const string COLLECTION_NAME = "PersonalInfo";
        private static PersonalDao instance { get; set; }
        public static PersonalDao GetInstance()
        {
            if (instance == null)
            {
                instance = new PersonalDao();
            }
            return instance;
        }

        public List<PersonalInfo> GetByConfigId(ObjectId configId)
        {

            var F = Builders<PersonalInfo>.Filter.Eq(p => p.ConfigId, configId);
            var rs = GetCollection().Find(F).ToList();
            return rs;
        }

        public PersonalDao() : base(COLLECTION_NAME) { }
    }
}
