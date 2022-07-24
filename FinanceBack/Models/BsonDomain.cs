using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceBack.Models
{
    public abstract class BsonDomain
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}
