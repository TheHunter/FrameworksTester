using System;
using System.ServiceModel;
using RabbitMQ.Client;
using RabbitMQ.ServiceModel;
using RabbitMqTester.model;
using RabbitMqTester.proxies;
using RabbitMqTester.services;

namespace RabbitMqTester
{
    /// <summary>
    /// Class WcfHost.
    /// </summary>
    public class WcfHost
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        static void Main(params string[] args)
        {
            WcfHost host = new WcfHost();
            host.Run();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            Console.WriteLine("Run a ServiceHost via programmatic configuration...");

            using (ServiceHost serviceHost = new ServiceHost(typeof(Calculator), new Uri("soap.amqp:///")))
            {
                var serviceEndpoint = serviceHost.AddServiceEndpoint(
                    typeof (ICalculator),
                    new RabbitMQBinding(
                        "localhost",
                        5672,
                        "guest",
                        "guest",
                        "/",
                        8192,
                        Protocols.AMQP_0_9_1)
                    {
                        OneWayOnly = false
                    },
                    "Calculator");

                ////serviceEndpoint.Behaviors.Add(new RabbitMqEndpointBehavior(null));
                Console.WriteLine("Num. behaviors: {0}", serviceEndpoint.Behaviors.Count);

                serviceHost.Open();
                Console.WriteLine("The service is opened, press ENTER to follow next instructions...");
                ////Console.ReadLine();

                var proxyBinding = new RabbitMQBinding(
                        "localhost",
                        5672,
                        "guest",
                        "guest",
                        "/",
                        8192,
                        Protocols.AMQP_0_9_1)
                {
                    OneWayOnly = false
                }
                ;
                var endpoint = new EndpointAddress("soap.amqp:///Calculator");

                using (var proxy = new CalculatorProxy(proxyBinding, endpoint))
                {
                    ////proxy.Endpoint.Behaviors.Add(new RabbitMqEndpointBehavior(null));
                    var res = proxy.Add(4, 5);
                    Console.WriteLine(res);

                    Console.WriteLine("Type name: {0}", proxy.PrintTypeName(new Person()));
                    Console.WriteLine("Type name: {0}", proxy.PrintTypeName(new Student()));
                }
                Console.ReadLine();
            }
        }
    }
}
