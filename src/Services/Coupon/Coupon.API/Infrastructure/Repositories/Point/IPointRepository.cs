namespace Coupon.API.Infrastructure.Repositories.Point;

public interface IPointRepository
{
    Task<Models.Point> GetPointsByUserId(string userId);
    Task<Models.Point> CreatePointsBalanceByUserId(string userId);
    Task AddPointsToBalanceByUser(string id, string userId, decimal balance);
    Task SubtractPointsFromBalanceByUser(string id, string userId, int points);
}