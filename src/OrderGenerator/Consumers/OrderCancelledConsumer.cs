using Common.Logging;
using MassTransit;
using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.Generator.Consumers
{
    public class OrderCancelledConsumer : IConsumer<IOrderCancelled>
    {
        private readonly ILog logger;

        public OrderCancelledConsumer()
        {
            this.logger = LogManager.GetLogger<OrderCancelledConsumer>();
        }

        public async Task Consume(ConsumeContext<IOrderCancelled> context)
        {
            this.logger.Info($"The order cancelled to {context.Message.CorrelationId} was received");
        }
    }
}
