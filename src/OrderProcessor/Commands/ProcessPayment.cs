using SagasDemo.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.OrderProcessor.Commands
{
    public class ProcessPayment : IProcessPayment
    {
        public ProcessPayment(Guid correlationId, Order order)
        {
            this.CorrelationId = correlationId;
            this.Order = order;
        }
        public Guid CorrelationId { get; }

        public Order Order { get; }
    }
}
