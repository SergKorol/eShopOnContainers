using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events;

public record DiscountBalanceConfirmedIntegrationEvent(string UserId, decimal Balance) : IntegrationEvent
{
    public string UserId { get; set; } = UserId;
    public decimal Balance { get; set; } = Balance;
}