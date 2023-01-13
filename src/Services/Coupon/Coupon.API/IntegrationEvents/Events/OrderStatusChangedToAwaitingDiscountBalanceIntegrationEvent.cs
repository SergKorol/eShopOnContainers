using Coupon.API.Enums;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;

namespace Coupon.API.IntegrationEvents.Events;

public sealed record OrderStatusChangedToAwaitingDiscountBalanceIntegrationEvent : IntegrationEvent
{
    [JsonProperty]
    public string UserId { get; set; }
    [JsonProperty]
    public int OrderId { get; private set; }

    [JsonProperty]
    public string OrderStatus { get; private set; }

    [JsonProperty]
    public string BuyerName { get; private set; }
    
    [JsonProperty]
    public decimal Balance { get; private set; }
}