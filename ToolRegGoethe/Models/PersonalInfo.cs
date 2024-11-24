using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToolRegGoethe.Models
{
    public class PersonalInfo : BaseModelInfo
    {
        public ObjectId ConfigId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsReading { get; set; }
        public bool IsListening { get; set; }
        public bool IsWriting { get; set; }
        public bool IsSpeaking { get; set; }
        public bool IsSuccess { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }

        [BsonIgnore]
        public string ConfigIdStr
        {
            get
            {
                return ConfigId.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ConfigId = new ObjectId(value);
                }
            }
        }
    }
}
