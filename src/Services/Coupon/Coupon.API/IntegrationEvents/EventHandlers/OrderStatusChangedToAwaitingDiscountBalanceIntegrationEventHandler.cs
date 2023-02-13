using Coupon.API.Enums;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.Infrastructure.Repositories.Point;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Serilog;
using Serilog.Context;

namespace Coupon.API.IntegrationEvents.EventHandlers;

public sealed class OrderStatusChangedToAwaitingDiscountBalanceIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent>
{
        private readonly IPointRepository _pointRepository;
        private readonly IEventBus _eventBus;

        public OrderStatusChangedToAwaitingDiscountBalanceIntegrationEventHandler(IPointRepository pointRepository, IEventBus eventBus)
        {
            _pointRepository = pointRepository;
            _eventBus = eventBus;
        }

        public async Task Handle(OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent @event)
        {
            await Task.Delay(3000);

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-Coupon.API"))
            {
                var pointIntegrationEvent = await ProcessIntegrationEventAsync(@event);

                Log.Information("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", pointIntegrationEvent.Id, "Coupon.API", pointIntegrationEvent);

                _eventBus.Publish(pointIntegrationEvent);
            }
        }

        private async Task<IntegrationEvent> ProcessIntegrationEventAsync(OrderStatusChangedToAwaitingDiscountBalanceValidationIntegrationEvent validationIntegrationEvent)
        {
            var balance = await _pointRepository.GetPointsByUserId(validationIntegrationEvent.BuyerName);

            Log.Information("----- Balance {Balance}", balance);

            if (balance.NumberOfPoints == default && validationIntegrationEvent.Balance <= 0)
            {
                return new OrderDiscountBalanceRejectedIntegrationEvent(validationIntegrationEvent.OrderId, validationIntegrationEvent.Balance);
            }
            
            await _pointRepository.AddPointsToBalanceByUser(balance.Id, validationIntegrationEvent.BuyerName, validationIntegrationEvent.Balance);
            
            return new OrderDiscountBalanceConfirmedIntegrationEvent(validationIntegrationEvent.OrderId, validationIntegrationEvent.Balance);
        }
}