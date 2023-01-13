using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string OrderStatus { get; }

    public string BuyerName { get; }

    public decimal Discount { get; set; }

    public OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent(int orderId, string orderStatus, string buyerName, decimal discount)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        Discount = discount;
    }
}