using Coupon.API.Infrastructure.Models;
using Coupon.API.Infrastructure.Repositories.Point;

namespace Coupon.API.Infrastructure;

public sealed class PointSeed
{
    public async Task SeedAsync(PointContext context)
    {
        if (await context.Points.EstimatedDocumentCountAsync() == 0)
        {
            var points = new List<Point>
            {
                new Point("demouser@microsoft.com", 10)
            };

            await context.Points.InsertManyAsync(points);
        }
    }
}