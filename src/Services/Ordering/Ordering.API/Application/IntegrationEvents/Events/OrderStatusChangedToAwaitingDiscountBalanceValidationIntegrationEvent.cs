using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string OrderStatus { get; }

    public string BuyerName { get; }

    public decimal Discount { get; set; }
    public decimal Balance { get; set; }

    public OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent(int orderId, string orderStatus, string buyerName, decimal discount, decimal balance)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        Discount = discount;
        Balance = balance;
    }
}