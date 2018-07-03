using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.OrderProcessor.Events
{
    [Serializable]
    public class OrderProcessed : IOrderProcessed
    {
        public OrderProcessed(Guid correlationId, Order order)
        {
            this.CorrelationId = correlationId;
            this.Order = order;
        }
        public Guid CorrelationId { get; }

        public Order Order { get; }
    }
}
