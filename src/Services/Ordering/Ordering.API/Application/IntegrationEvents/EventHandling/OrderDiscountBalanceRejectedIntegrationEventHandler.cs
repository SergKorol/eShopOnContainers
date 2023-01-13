using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationEvents.Events;
using Serilog;
using Serilog.Context;

namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderDiscountBalanceRejectedIntegrationEventHandler : IIntegrationEventHandler<OrderDiscountBalanceRejectedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderDiscountBalanceRejectedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderDiscountBalanceRejectedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            Log.Warning("Discount failed, cancelling order {OrderId}", @event.OrderId);

            var command = new CancelOrderCommand(@event.OrderId);

            Log.Information("----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                command.GetGenericTypeName(),
                nameof(command.OrderNumber),
                command.OrderNumber,
                command);

            await _mediator.Send(command);
        }
    }
}