using Common.Logging;
using MassTransit;
using SagasDemo.Commands;
using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.Warehouse.Consumers
{
    public class ReserveStockConsumer : IConsumer<IReserveStock>
    {
        private readonly ILog logger;

        public ReserveStockConsumer()
        {
            this.logger = LogManager.GetLogger<ReserveStockConsumer>();
        }
        public async Task Consume(ConsumeContext<IReserveStock> context)
        {
            this.logger.Info($"Reserve stock to {context.Message.CorrelationId} was received");
            await Task.Delay(2000);
            this.UpdateOrderState(context.Message.Order);
            await context.Publish<IStockReserved>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Order = context.Message.Order
            });
        }

        private void UpdateOrderState(Order order) =>
         order.Status = Status.StockReserved;
    }
}
