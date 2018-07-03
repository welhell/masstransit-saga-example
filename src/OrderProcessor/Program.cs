using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using System;
using System.ServiceProcess;

namespace SagasDemo.OrderProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting orden processor");
            ConfigureLogger();
            var service = new OrderProcessorService();
            service.Start();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
            service.Stop();
        }

        private static void ConfigureLogger()
        {
            var settings =  new NameValueCollection
                {
                        { "configType", "FILE" },
                        { "configFile", "~/OrderProcessor.exe.nlog" }
                };
            LogManager.Adapter = new NLogLoggerFactoryAdapter(settings);
            MassTransit.NLogIntegration.Logging.NLogLogger.Use();
        }
    }
}
