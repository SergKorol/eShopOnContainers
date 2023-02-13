using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationEvents.Events;
using Serilog;
using Serilog.Context;

namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderDiscountBalanceConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderDiscountBalanceConfirmedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderDiscountBalanceConfirmedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderDiscountBalanceConfirmedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var command = new DiscountBalanceConfirmedCommand(@event.OrderId, @event.Balance);
            Console.WriteLine("FUCK CONFIRM");
            Log.Information("----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                command.GetGenericTypeName(),
                nameof(command.OrderNumber),
                command.OrderNumber,
                command);

            await _mediator.Send(command);
        }
    }
}