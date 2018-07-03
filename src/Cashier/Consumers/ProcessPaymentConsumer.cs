using Common.Logging;
using MassTransit;
using SagasDemo.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.Cashier.Consumers
{
    public class ProcessPaymentConsumer:IConsumer<IProcessPayment>
    {
        private readonly ILog logger;
        public ProcessPaymentConsumer()
        {
            this.logger = LogManager.GetLogger<ProcessPaymentConsumer>();
        }
        public async Task Consume(ConsumeContext<IProcessPayment> context)
        {
            this.logger.Info($"Process payment command receives to {context.Message.CorrelationId}");
            await Task.Delay(2000);
            this.UpdateOrderState(context.Message.Order);
            await context.Publish<IPaymentProcessed>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Order = context.Message.Order
            });
        }

        private void UpdateOrderState(Order order) =>
            order.Status = Status.Paymented;
        
    }
}
