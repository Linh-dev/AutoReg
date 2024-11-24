using MongoDB.Driver;

namespace ToolRegGoethe.Dao
{
    public class Database
    {
        private static MongoClient client;
        public static IMongoDatabase GetDatabase()
        {
            if (client == null)
            {
                var mongoUrl = "mongodb://localhost:27017/ToolRegGoethe";
                //var mongoUrl = BusinessSettings.MongoDBConnectionStrings;
                client = new MongoClient(mongoUrl);
            }

            return client.GetDatabase("ToolRegGoethe");
        }
    }
}
