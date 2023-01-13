using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Coupon.API.Infrastructure.Models
{
    public class Coupon
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public decimal Discount { get; set; }

        public string Code { get; set; }

        public bool Consumed { get; set; }

        public int OrderId { get; set; }
    }
}
