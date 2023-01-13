using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderCouponConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public decimal Discount { get; }

        public OrderCouponConfirmedIntegrationEvent(int orderId, decimal discount)
        {
            OrderId = orderId;
            Discount = discount;
        }
    }
}
