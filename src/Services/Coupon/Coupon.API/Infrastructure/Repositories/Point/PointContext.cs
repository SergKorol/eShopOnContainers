using Coupon.API.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Coupon.API.Infrastructure.Repositories.Point;

public sealed class PointContext
{
    private readonly IMongoDatabase? _database;

    public PointContext(IOptions<PointSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);

        if (client is null)
        {
            throw new MongoConfigurationException("Cannot connect to the database. The connection string is not valid or the database is not accessible");
        }

        _database = client.GetDatabase(settings.Value.MongoDatabase);
    }
    
    public IMongoCollection<Models.Point>? Points => _database?.GetCollection<Models.Point>("PointCollection");
}