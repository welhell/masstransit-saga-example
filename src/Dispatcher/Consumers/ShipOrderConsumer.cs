using Common.Logging;
using MassTransit;
using SagasDemo.Commands;
using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.Dispatcher.Consumers
{
    public class ShipOrderConsumer : IConsumer<IShipOrder>
    {
        private readonly ILog logger;

        public ShipOrderConsumer()
        {
            this.logger = LogManager.GetLogger<ShipOrderConsumer>();
        }
        public async Task Consume(ConsumeContext<IShipOrder> context)
        {
            logger.Info($"Shipt order received {context.Message.CorrelationId}");
            await Task.Delay(2000);
            this.UpdateOrderState(context.Message.Order);
            await context.Publish<IOrderShipped>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Order = context.Message.Order
            });
        }

        private void UpdateOrderState(Order order) =>
           order.Status = Status.Shipped;
    }
}
