
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace Takasbu.Models
{
    public class User
    {
       [BsonId]
       [BsonRepresentation(BsonType.ObjectId)]
       public string Id { get; internal set; } = string.Empty;

        [BsonElement("Picture")]
        [JsonPropertyName("Picture")]
        public string Picture{ get; set; } = string.Empty;
       

       [BsonElement("Name")]
       [JsonPropertyName("Name")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("Bio")]
       [JsonPropertyName("Bio")]
        public string Bio { get; set; } = string.Empty;
        [BsonElement("ProductIds")]
        public List<string> ProductIds { get; set; } 
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public User()
    {
        ProductIds = new List<string>();
    }
        
    }
}