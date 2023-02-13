using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events;

public record OrderDiscountBalanceRejectedIntegrationEvent(int OrderId, decimal Balance) : IntegrationEvent
{
    public int OrderId { get; } = OrderId;
    public decimal Balance { get; set; } = Balance;
}