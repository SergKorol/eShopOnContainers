﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using Ordering.Domain.Events;
using Ordering.Domain.Exceptions;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order
        : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private DateTime _orderDate;

        // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
        public Address Address { get; private set; }

        public int? GetBuyerId => _buyerId;
        private int? _buyerId;

        public OrderStatus OrderStatus { get; private set; }

        public bool? DiscountConfirmed { get; private set; }

        private int _orderStatusId;

        private string _description;

        public string DiscountCode { get; private set; }

        public decimal? Discount { get; private set; }
        public decimal? Balance { get; private set; }

        // Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order, but we could do it if needed
#pragma warning disable CS0414
        private bool _isDraft;
#pragma warning restore CS0414

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        private int? _paymentMethodId;

        public static Order NewDraft()
        {
            var order = new Order();
            order._isDraft = true;
            return order;
        }

        protected Order()
        {
            _orderItems = new List<OrderItem>();
            _isDraft = false;
        }

        public Order(string userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
                string cardHolderName, DateTime cardExpiration, string discountCode, decimal? discount, decimal? balance, int? buyerId = null, int? paymentMethodId = null) : this()
        {
            _buyerId = buyerId;
            _paymentMethodId = paymentMethodId;
            _orderStatusId = OrderStatus.Submitted.Id;
            _orderDate = DateTime.UtcNow;
            Address = address;
            DiscountCode = discountCode;
            Discount = discountCode == null ? null : discount;
            Balance = balance == null ? 0 : balance;

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
                                       cardSecurityNumber, cardHolderName, cardExpiration);
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 
        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
                .SingleOrDefault();

            if (existingOrderForProduct != null)
            {
                //if previous line exist modify it with higher discount  and units..

                if (discount > existingOrderForProduct.GetCurrentDiscount())
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                }

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                //add validated new order item

                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                _orderItems.Add(orderItem);
            }
        }

        public void SetPaymentId(int id)
        {
            _paymentMethodId = id;
        }

        public void SetBuyerId(int id)
        {
            _buyerId = id;
        }

        public void SetAwaitingStockValidationStatus()
        {
            if (_orderStatusId != OrderStatus.Submitted.Id)
            {
                StatusChangeException(OrderStatus.AwaitingStockValidation);
            }

            _orderStatusId = OrderStatus.AwaitingStockValidation.Id;

            AddDomainEvent(new OrderStatusChangedToAwaitingStockValidationDomainEvent(Id, _orderItems));
        }

        public void ProcessStockConfirmed()
        {
            // If there's no Couponm, then it's validated
            if (DiscountCode == null)
            {
                if (Balance != null)
                {
                    
                    if (_orderStatusId != OrderStatus.AwaitingStockValidation.Id)
                    {
                        StatusChangeException(OrderStatus.AwaitingCouponValidation);
                    }

                    _orderStatusId = Discount != null ? OrderStatus.AwaitingCouponValidation.Id : OrderStatus.Validated.Id;
                    _description = "Validate discount balance";
                    AddDomainEvent(new OrderStatusChangedToAwaitingDiscountBalanceValidationDomainEvent(Id, Balance.Value));
                }
                else
                {
                    if (_orderStatusId != OrderStatus.AwaitingStockValidation.Id)
                    {
                        StatusChangeException(OrderStatus.Validated);
                    }

                    _orderStatusId = OrderStatus.Validated.Id;
                    _description = "All the items were confirmed with available stock.";

                    AddDomainEvent(new OrderStatusChangedToValidatedDomainEvent(Id));
                }
            }
            else
            {
                if (_orderStatusId != OrderStatus.AwaitingStockValidation.Id)
                {
                    StatusChangeException(OrderStatus.AwaitingCouponValidation);
                }

                _orderStatusId = OrderStatus.AwaitingCouponValidation.Id;
                _description = "Validate discount code";

                AddDomainEvent(new OrderStatusChangedToAwaitingCouponValidationDomainEvent(Id, DiscountCode));
            }
        }

        public void ProcessCouponConfirmed()
        {
            if (_orderStatusId != OrderStatus.AwaitingCouponValidation.Id)
            {
                StatusChangeException(OrderStatus.Validated);
            }

            DiscountConfirmed = true;

            _orderStatusId = OrderStatus.Validated.Id;
            _description = "Discount coupon validated.";
            Console.WriteLine($"MY STATUS COUP: {_orderStatusId}");
            AddDomainEvent(new OrderStatusChangedToValidatedDomainEvent(Id));
        }
        
        public void ProcessDiscountBalanceConfirmed()
        {
            Console.WriteLine($"MY STATUS BAL: {_orderStatusId}");
            if (_orderStatusId != OrderStatus.Validated.Id)
            {
                StatusChangeException(OrderStatus.Paid);
            }

            // DiscountConfirmed = true;

            _orderStatusId = OrderStatus.Paid.Id;
            _description = "Discount balance validated.";

            AddDomainEvent(new OrderStatusChangedToValidatedDomainEvent(Id));
        }

        public void SetPaidStatus()
        {
            if (_orderStatusId != OrderStatus.Validated.Id)
            {
                StatusChangeException(OrderStatus.Paid);
            }

            _orderStatusId = OrderStatus.Paid.Id;
            _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";

            AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));
        }

        public void SetShippedStatus()
        {
            if (_orderStatusId != OrderStatus.Paid.Id)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            _orderStatusId = OrderStatus.Shipped.Id;
            _description = "The order was shipped.";

            AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public void SetCancelledStatus()
        {
            if (_orderStatusId == OrderStatus.Paid.Id ||
                _orderStatusId == OrderStatus.Shipped.Id)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            _orderStatusId = OrderStatus.Cancelled.Id;
            _description = $"The order was cancelled.";

            AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (_orderStatusId == OrderStatus.AwaitingStockValidation.Id)
            {
                _orderStatusId = OrderStatus.Cancelled.Id;

                var itemsStockRejectedProductNames = OrderItems
                    .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                    .Select(c => c.GetOrderItemProductName());

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
                _description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
            }
        }

        private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
                string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
                                                                      cardNumber, cardSecurityNumber,
                                                                      cardHolderName, cardExpiration);

            this.AddDomainEvent(orderStartedDomainEvent);
        }

        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new OrderingDomainException($"It isn't possible to change the status for order \"{Id}\" from \"{OrderStatus.Name}\" to \"{orderStatusToChange.Name}\"!");
        }

        public decimal GetTotal()
        {
            var result = _orderItems.Sum(o => o.GetUnits() * o.GetUnitPrice()) - (Discount ?? 0);

            return result < 0 ? 0 : result;
        }
    }
}

