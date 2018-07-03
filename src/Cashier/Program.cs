using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using MassTransit;
using MassTransit.Util;
using SagasDemo.Cashier.Consumers;
using SagasDemo.Commands;
using System;
using System.Configuration;

namespace SagasDemo.Cashier
{
    class Program
    { 
        static void Main(string[] args)
        {
            Console.WriteLine("Starting cashier...");
            ConfigureLogger();
            ConfigureAndStartBus();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
        private static void ConfigureLogger()
        {
            var settings = new NameValueCollection
                {
                        { "configType", "FILE" },
                        { "configFile", "~/Cashier.exe.nlog" }
                };
            LogManager.Adapter = new NLogLoggerFactoryAdapter(settings);
            MassTransit.NLogIntegration.Logging.NLogLogger.Use();
        }
        private static void ConfigureAndStartBus()
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
                    c.Consumer(() => new ProcessPaymentConsumer());
                });
            });
            TaskUtil.Await<BusHandle>(() => bus.StartAsync());
        }

       


    }
}
