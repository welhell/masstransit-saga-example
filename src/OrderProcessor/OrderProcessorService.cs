using Common.Logging;
using MassTransit;
using MassTransit.MongoDbIntegration.Saga;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using MassTransit.Util;
using MongoDB.Driver;
using SagasDemo.OrderProcessor.Services;
using SagasDemo.OrderProcessor.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using System.Text;

namespace SagasDemo.OrderProcessor
{
    public class OrderProcessorService
    {
        private const int MAX_NUMBER_OF_PROCESSING_MESSAGES = 8;
        private readonly ILog logger;
        private IBusControl busControl;
        private BusHandle busHandler;
        public OrderProcessorService()
        {
          logger = LogManager.GetLogger< OrderProcessorService>();

        }

        public void Start()
        {
            logger.Info("Starting bus");
            (this.busControl, this.busHandler) = this.CreateBus();
        }
      

        private (IBusControl, BusHandle) CreateBus()
        { 
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(this.ConfigureBus);
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            return (bus, busHandle);
        }

        private void ConfigureBus(IRabbitMqBusFactoryConfigurator factoryConfigurator)
        {
            var rabbitHost = new Uri(ConfigurationManager.AppSettings["rabbitHost"]);
            var inputQueue = ConfigurationManager.AppSettings["rabbitInputQueue"];
            var host = factoryConfigurator.Host(rabbitHost, this.ConfigureCredentials);
            factoryConfigurator.ReceiveEndpoint(host, inputQueue, this.ConfigureSagaEndpoint);
        }
        private void ConfigureCredentials(IRabbitMqHostConfigurator hostConfiurator)
        {
            var user = ConfigurationManager.AppSettings["rabbitUser"];
            var password = ConfigurationManager.AppSettings["rabbitPassword"];
            hostConfiurator.Username(user);
            hostConfiurator.Password(password);            
        }

        private void ConfigureSagaEndpoint(IRabbitMqReceiveEndpointConfigurator endPointConfigurator)
        {
            var stateMachine = new OrderProcessorStateMachine();
            var repository = this.CreateRepository();
          //  endPointConfigurator.PrefetchCount = MAX_NUMBER_OF_PROCESSING_MESSAGES;
            endPointConfigurator.StateMachineSaga(stateMachine, repository);        
        }

        private ISagaRepository<ProcessingOrderState> CreateRepository()
        {
            var mongoServer = ConfigurationManager.AppSettings["mongoHost"];
            var databaseName = ConfigurationManager.AppSettings["mongoDatabase"];
            return new MongoDbSagaRepository<ProcessingOrderState>(mongoServer,databaseName);
        }
           
        public void Stop()
        {
            logger.Info("Stopping bus");
            this.TryToStopBus(); 
        }        

        private void TryToStopBus() =>
            this.busHandler?.Stop();
    }
}
