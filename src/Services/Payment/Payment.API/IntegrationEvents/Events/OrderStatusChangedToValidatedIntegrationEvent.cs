﻿namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Newtonsoft.Json;

    public record OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        [JsonProperty]
        public int OrderId { get; private set; }

        [JsonProperty]
        public decimal Total { get; private set; }
    }
}