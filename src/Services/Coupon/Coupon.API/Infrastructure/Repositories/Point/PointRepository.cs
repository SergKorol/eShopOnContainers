using MongoDB.Driver;

namespace Coupon.API.Infrastructure.Repositories.Point;

public sealed class PointRepository : IPointRepository
{
    private readonly PointContext _pointContext;
    public PointRepository(PointContext context)
    {
        _pointContext = context;
    }

    public async Task<Models.Point> GetPointsByUserId(string BuyerName)
    {
        var filter = Builders<Models.Point>.Filter.Eq("UserId", BuyerName);
        return await _pointContext.Points.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Models.Point> CreatePointsBalanceByUserId(string BuyerName)
    {
        var filter = Builders<Models.Point>.Filter.Eq("UserId", BuyerName);
        var point = await _pointContext.Points.Find(filter).FirstOrDefaultAsync();
        if (point is null)
        {
            var update = Builders<Models.Point>.Update
                .Set(point => point.NumberOfPoints, default)
                .Set(point => point.UserId, BuyerName);
            await _pointContext.Points.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        return point;
    }
    
    public async Task AddPointsToBalanceByUser(string id, string BuyerName, decimal balance)
    {
        var filter = Builders<Models.Point>.Filter.Eq("Id", id);
        var point = await _pointContext.Points.Find(filter).FirstOrDefaultAsync();
        var update = Builders<Models.Point>.Update
            .Set(point => point.NumberOfPoints, point.NumberOfPoints + (int)balance)
            .Set(point => point.Cash, point.Cash + (double)balance)
            .Set(point => point.UserId, BuyerName);
        await _pointContext.Points.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
    }
    
    public async Task SubtractPointsFromBalanceByUser(string id, string userId, int points)
    {
        var filter = Builders<Models.Point>.Filter.Eq("Id", id);
        var point = await _pointContext.Points.Find(filter).FirstOrDefaultAsync();
        var update = Builders<Models.Point>.Update
            .Set(point => point.NumberOfPoints, point.NumberOfPoints - points)
            .Set(point => point.UserId, point.UserId);
        await _pointContext.Points.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
    }
}