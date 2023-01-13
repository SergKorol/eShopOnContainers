namespace Ordering.Domain.Events;

public class OrderStatusChangedToAwaitingDiscountBalanceValidationDomainEvent : INotification
{
    public int OrderId { get; }

    public decimal Balance { get; set; }

    public OrderStatusChangedToAwaitingDiscountBalanceValidationDomainEvent(int orderId, decimal balance)
    {
        OrderId = orderId;
        Balance = balance;
    }
}