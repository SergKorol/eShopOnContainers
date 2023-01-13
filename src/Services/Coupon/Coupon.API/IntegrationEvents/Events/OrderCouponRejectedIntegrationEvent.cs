namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Newtonsoft.Json;

    public record OrderCouponRejectedIntegrationEvent(int IntegrationEventOrderId, string CouponCode) : IntegrationEvent
    {
        [JsonProperty]
        public int OrderId { get; private set; }

        [JsonProperty]
        public string Code { get; private set; }
    }
}