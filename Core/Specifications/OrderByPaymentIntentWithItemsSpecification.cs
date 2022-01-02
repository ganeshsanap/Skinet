using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Specifications
{
    public class OrderByPaymentIntentWithItemsSpecification : BaseSpecifcation<Order>
    {
        public OrderByPaymentIntentWithItemsSpecification(string paymentIntentId) : base(o => o.PaymentIntentId == paymentIntentId)
        {
        }
    }
}
