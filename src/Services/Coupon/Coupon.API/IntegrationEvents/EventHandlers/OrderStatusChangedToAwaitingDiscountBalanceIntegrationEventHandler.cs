using Coupon.API.Enums;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.Infrastructure.Repositories.Point;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Serilog;
using Serilog.Context;

namespace Coupon.API.IntegrationEvents.EventHandlers;

public sealed class OrderStatusChangedToAwaitingDiscountBalanceIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingDiscountBalanceIntegrationEvent>
{
        private readonly IPointRepository _pointRepository;
        private readonly IEventBus _eventBus;

        public OrderStatusChangedToAwaitingDiscountBalanceIntegrationEventHandler(IPointRepository pointRepository, IEventBus eventBus)
        {
            _pointRepository = pointRepository;
            _eventBus = eventBus;
        }

        public async Task Handle(OrderStatusChangedToAwaitingDiscountBalanceIntegrationEvent @event)
        {
            await Task.Delay(3000);

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-Coupon.API"))
            {
                Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "Coupon.API", @event);

                var pointIntegrationEvent = await ProcessIntegrationEventAsync(@event);

                Log.Information("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", pointIntegrationEvent.Id, "Coupon.API", pointIntegrationEvent);

                _eventBus.Publish(pointIntegrationEvent);
            }
        }

        private async Task<IntegrationEvent> ProcessIntegrationEventAsync(OrderStatusChangedToAwaitingDiscountBalanceIntegrationEvent integrationEvent)
        {
            var balance = await _pointRepository.GetPointsByUserId(integrationEvent.UserId);
            if (balance is null)
            {
                balance = await _pointRepository.CreatePointsBalanceByUserId(integrationEvent.UserId);
            }
            Log.Information("----- Balance \"{UserId}\": {@UserId}", integrationEvent.UserId, balance);

            if (balance.NumberOfPoints == default && integrationEvent.Balance < 0)
            {
                return new DiscountBalanceRejectedIntegrationEvent(integrationEvent.UserId, integrationEvent.Balance);
            }
            
            Log.Information("Updated balance: {NumberOfPoints}", integrationEvent.Balance);

            await _pointRepository.AddPointsToBalanceByUser(balance.Id, integrationEvent.UserId, integrationEvent.Balance);
            
            
            return new DiscountBalanceConfirmedIntegrationEvent(integrationEvent.UserId, integrationEvent.Balance);
        }
}