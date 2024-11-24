using MongoDB.Driver;
using ToolRegGoethe.Models;

namespace ToolRegGoethe.Dao
{
    public class ConfigDao: BaseDao<ConfigInfo>
    {
        const string COLLECTION_NAME = "ConfigInfo";
        private static ConfigDao instance { get; set; }
        public static ConfigDao GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigDao();
            }
            return instance;
        }
        public ConfigDao() : base(COLLECTION_NAME) { }
    }
}
