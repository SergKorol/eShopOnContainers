﻿using System.Runtime.Serialization;
using MediatR;

namespace Ordering.API.Application.Commands
{
    public class CouponConfirmedCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        [DataMember]
        public decimal Discount { get; private set; }

        public CouponConfirmedCommand(int orderNumber, decimal discount)
        {
            OrderNumber = orderNumber;
            Discount = discount;
        }
    }
}