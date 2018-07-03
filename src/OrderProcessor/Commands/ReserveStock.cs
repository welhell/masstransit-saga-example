using SagasDemo.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.OrderProcessor.Commands
{
    public class ReserveStock : IReserveStock
    {
        public ReserveStock(Guid correlationId, Order order)
        {
            this.CorrelationId = correlationId;
            this.Order = order;
        }
        public Guid CorrelationId { get; }

        public Order Order { get; }
    }
}
