namespace Coupon.API.Infrastructure.Repositories.Point;

public interface IPointRepository
{
    Task<Models.Point> GetPointsByUserId(string BuyerName);
    Task<Models.Point> CreatePointsBalanceByUserId(string BuyerName);
    Task AddPointsToBalanceByUser(string id, string BuyerName, decimal balance);
    Task SubtractPointsFromBalanceByUser(string id, string userId, int points);
}