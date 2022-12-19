namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public IEnumerable<OrderStatusChangedToAwaitingValidationIntegrationEvent.OrderStockItem> OrderStockItems { get; }

    public OrderStatusChangedToPaidIntegrationEvent(int orderId,
        IEnumerable<OrderStatusChangedToAwaitingValidationIntegrationEvent.OrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
    }
}
