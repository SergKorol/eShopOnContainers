using System.Runtime.Serialization;
using MediatR;

namespace Ordering.API.Application.Commands;

public class DiscountBalanceConfirmedCommand : IRequest<bool>
{
    [DataMember]
    public int OrderNumber { get; private set; }

    [DataMember]
    public decimal Balance { get; private set; }

    public DiscountBalanceConfirmedCommand(int orderNumber, decimal balance)
    {
        OrderNumber = orderNumber;
        Balance = balance;
    }
}