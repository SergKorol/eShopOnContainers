using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.SignalrHub.IntegrationEvents.Events
{
    public record OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToValidatedIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
