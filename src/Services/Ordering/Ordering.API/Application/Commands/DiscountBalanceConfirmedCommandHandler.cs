using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Application.Commands;

public class DiscountBalanceConfirmedCommandHandler : IRequestHandler<DiscountBalanceConfirmedCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<DiscountBalanceConfirmedCommandHandler> _logger;

    public DiscountBalanceConfirmedCommandHandler(IOrderRepository orderRepository, ILogger<DiscountBalanceConfirmedCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DiscountBalanceConfirmedCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("!!!!!----- Creating Balance - Order: {@Balance}", command.Balance);
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);

        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.ProcessDiscountBalanceConfirmed();

        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
    
    public class DiscountBalanceConfirmIdenfifiedCommandHandler : IdentifiedCommandHandler<DiscountBalanceConfirmedCommand, bool>
    {
        public DiscountBalanceConfirmIdenfifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<DiscountBalanceConfirmedCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing order.
        }
    }
}