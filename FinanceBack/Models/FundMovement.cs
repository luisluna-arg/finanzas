using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceBack.Models
{
    public class FundMovement : BsonDomain
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("concept")]
        public string? Concept { get; set; }

        [BsonElement("concept2")]
        public string? Concept2 { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }
    }
}
