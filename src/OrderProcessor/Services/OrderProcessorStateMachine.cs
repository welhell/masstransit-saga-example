using Automatonymous;
using Automatonymous.Binders;
using Common.Logging;
using SagasDemo.Commands;
using SagasDemo.Events;
using SagasDemo.OrderProcessor.Events;
using SagasDemo.OrderProcessor.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using SagaState = Automatonymous.State;
namespace SagasDemo.OrderProcessor.Services
{
    public class OrderProcessorStateMachine : MassTransitStateMachine<ProcessingOrderState>
    {
        private readonly ILog logger;

        public OrderProcessorStateMachine()
        {
            this.logger = LogManager.GetLogger<OrderProcessorStateMachine>();
            this.InstanceState(x => x.State);
            this.State(() => this.Processing);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetOrderSummitedHandler());
            this.During(Processing, this.SetStockReservedHandler(), SetPaymentProcessedHandler(), SetOrderShippedHandler());
            SetCompletedWhenFinalized();
        }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.OrderSubmitted, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.StockReserved, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.PaymentProcessed, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.OrderShipped, x => x.CorrelateById(c => c.Message.CorrelationId));
        }

        private EventActivityBinder<ProcessingOrderState, IOrderSubmitted> SetOrderSummitedHandler() =>
            When(OrderSubmitted).Then(c => this.UpdateSagaState(c.Instance, c.Data.Order))
                                .Then(c => this.logger.Info($"Order submitted to {c.Data.CorrelationId} received"))
                                .ThenAsync(c => this.SendCommand<IReserveStock>("rabbitWarehouseQueue", c))
                                .TransitionTo(Processing);


        private EventActivityBinder<ProcessingOrderState, IStockReserved> SetStockReservedHandler() =>
            When(StockReserved).Then(c => this.UpdateSagaState(c.Instance, c.Data.Order))
                               .Then(c => this.logger.Info($"Stock reserved to {c.Data.CorrelationId} received"))
                               .ThenAsync(c => this.SendCommand<IProcessPayment>("rabbitCashierQueue", c));


        private EventActivityBinder<ProcessingOrderState, IPaymentProcessed> SetPaymentProcessedHandler() =>
            When(PaymentProcessed).Then(c => this.UpdateSagaState(c.Instance, c.Data.Order))
                                  .Then(c => this.logger.Info($"Payment processed to {c.Data.CorrelationId} received"))
                                  .ThenAsync(c => this.SendCommand<IShipOrder>("rabbitDispatcherQueue", c));


        private EventActivityBinder<ProcessingOrderState, IOrderShipped> SetOrderShippedHandler() =>
            When(OrderShipped).Then(c =>
                                        {
                                            this.UpdateSagaState(c.Instance, c.Data.Order);
                                            c.Instance.Order.Status = Status.Processed;
                                        })
                              .Publish(c => new OrderProcessed(c.Data.CorrelationId, c.Data.Order))
                              .Finalize();

        private void UpdateSagaState(ProcessingOrderState state, Order order)
        {
            var currentDate = DateTime.Now;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Order = order;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProcessingOrderState, IMessage> context)
            where TCommand : class, IMessage
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings[endpointKey]));
            await sendEndpoint.Send<TCommand>(new
            {
                CorrelationId = context.Data.CorrelationId,
                Order = context.Data.Order
            });
        }
        public SagaState Processing { get; private set; }
        public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
        public Event<IOrderShipped> OrderShipped { get; set; }
        public Event<IPaymentProcessed> PaymentProcessed { get; private set; }
        public Event<IStockReserved> StockReserved { get; private set; }
    }
}
