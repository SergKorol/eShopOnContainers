using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;

namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderDiscountBalanceConfirmedIntegrationEvent : IntegrationEvent
{
    [JsonProperty]
    public int OrderId { get; private set; }

    [JsonProperty]
    public decimal Balance { get; private set; }
}