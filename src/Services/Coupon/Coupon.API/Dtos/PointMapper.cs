using Coupon.API.Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coupon.API.Dtos;

public class PointMapper : IMapper<PointDto, Point>
{
    public PointDto Translate(Point entity)
    {
        return new PointDto
        {
            NumberOfPoints = entity.NumberOfPoints,
            Cash = entity.Cash
        };
    }
}