using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Coupon.API.Infrastructure.Models;

public class Point
{
    [BsonIgnoreIfDefault]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public int NumberOfPoints { get; set; }

    private double _cash;
    
    public double Cash
    {
        get => (double)NumberOfPoints/100;
        set => _cash = value;
    }

    public Point(string userId, int numberOfPoints)
    {
        UserId = userId;
        NumberOfPoints = numberOfPoints;
    }
}