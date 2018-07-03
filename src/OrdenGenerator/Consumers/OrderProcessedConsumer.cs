using Common.Logging;
using MassTransit;
using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.OrdenGenerator.Consumers
{
    public class OrderProcessedConsumer : IConsumer<IOrderProcessed>
    {
        private readonly ILog logger;

        public OrderProcessedConsumer()
        {
            this.logger = LogManager.GetLogger<OrderProcessedConsumer>();
        }

        public async Task Consume(ConsumeContext<IOrderProcessed> context)
        {
            this.logger.Info($"Order processed to {context.Message.CorrelationId} was received");
        }
    }
}
