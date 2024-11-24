using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ToolRegGoethe.Utilities;

namespace ToolRegGoethe.Models
{
    public class ConfigInfo : BaseModelInfo
    {
        public string Name { get; set; }
        public string Link { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? StartTime { get; set; }
        [BsonIgnore]
        public string StartTimeStr
        {
            get
            {
                return DateUtil.DateTimeToString(StartTime);
            }
            set
            {
                StartTime = DateUtil.StringToDateTime(value);
            }
        }
        [BsonIgnore]
        public int CountPersonal { get; set; }
        public int CountSuccess { get; set; }
        public int CountFailure { get; set; }
    }
}
