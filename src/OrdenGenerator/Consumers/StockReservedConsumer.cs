using MassTransit;
using SagasDemo.Commands;
using SagasDemo.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagasDemo.OrdenGenerator.Consumers
{
    public class StockReservedConsumer : IConsumer<IStockReserved>
    {
        public async Task Consume(ConsumeContext<IStockReserved> context)
        {
            Console.WriteLine($"StockReserved event to  {context.Message.CorrelationId} receives");
        }
    }
}
