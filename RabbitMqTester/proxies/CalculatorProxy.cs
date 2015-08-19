using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using RabbitMqTester.model;
using RabbitMqTester.services;

namespace RabbitMqTester.proxies
{
    public class CalculatorProxy
        : ClientBase<ICalculator>, ICalculator
    {
        public CalculatorProxy(string configName)
            : base(configName)
        {
            
        }

        public CalculatorProxy(Binding binding, EndpointAddress endpoint)
            : base(binding, endpoint)
        {

        }

        public int Add(int x, int y)
        {
            //return this.Channel.Add(x, y);
            var res = this.Channel.Add(x, y);
            return res;
        }

        public int Subtract(int x, int y)
        {
            return this.Channel.Subtract(x, y);
        }

        public string PrintTypeName(Person person)
        {
            return this.Channel.PrintTypeName(person as Person);
        }
    }
}
