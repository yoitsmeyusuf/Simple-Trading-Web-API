using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Takasbu.Models
{
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public  string Id { get; set; } = string.Empty;

    [BsonElement("Name")]
     [JsonPropertyName("Name")]
    public  string Name { get; set; } = string.Empty;

    [BsonElement("Price")]
     [JsonPropertyName("Price")]
    public decimal Price { get; set; }

    [BsonElement("Category")]
     [JsonPropertyName("Category")]
    public  string Category { get; set; } = string.Empty;

    [BsonElement("Description")]
       [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("Owner")]
    public  string Owner { get; set; } = string.Empty;
}


}
