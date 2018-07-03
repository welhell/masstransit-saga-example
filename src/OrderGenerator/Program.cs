using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using MassTransit;
using MassTransit.Util;
using SagasDemo.Commands;
using SagasDemo.Events;
using SagasDemo.Generator.Services;
using SagasDemo.Generator.Consumers;
using System;
using System.Configuration;

namespace SagasDemo.Generator
{
    class Program
    {
        private static bool continueRunning = true;
        static void Main(string[] args)
        {
            ConfigureLogger();
            Console.CancelKeyPress += CancelKeyPressed;
            var bus = CreateBust();
            Console.WriteLine("Starting orden generator");
            Console.WriteLine("Enter to send..");
            Console.ReadLine();
            while (continueRunning)
            {             
                var order = OrderGenerator.Currrent.Generate();
                bus.Publish<IOrderSubmitted>(new { CorrelationId = Guid.NewGuid(), Order = order });
                Console.WriteLine(order.ToString());
                Console.ReadLine();
            }
        }
        private static void ConfigureLogger()
        {
            var settings = new NameValueCollection
                {
                        { "configType", "FILE" },
                        { "configFile", "~/OrderGenerator.exe.nlog" }
                };
            LogManager.Adapter = new NLogLoggerFactoryAdapter(settings);
            MassTransit.NLogIntegration.Logging.NLogLogger.Use();
        }

        private static void CancelKeyPressed(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            continueRunning = false;
        }
        private static IBus CreateBust()
        {
            var rabbitHost = new Uri(ConfigurationManager.AppSettings["rabbitHost"]);
            var user = ConfigurationManager.AppSettings["rabbitUser"];
            var password = ConfigurationManager.AppSettings["rabbitPassword"];
            var inputQueue = ConfigurationManager.AppSettings["rabbitInputQueue"];
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(configurator =>
            {                
                var host = configurator.Host(rabbitHost, h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                configurator.ReceiveEndpoint(host, inputQueue, c =>
                  {
                      c.Consumer(() => new OrderCancelledConsumer());
                      c.Consumer(() => new OrderProcessedConsumer());
                  });
            });

           TaskUtil.Await(() => bus.StartAsync());
            return bus;
        }

    }
}
