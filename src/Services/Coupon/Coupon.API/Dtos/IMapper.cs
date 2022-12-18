namespace Coupon.API.Dtos
{
    public interface IMapper<out TResult, in TEntity>
    {
        TResult Translate(TEntity entity);
    }
}
