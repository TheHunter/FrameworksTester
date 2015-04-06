using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using RabbitMqTester.model;

namespace RabbitMqTester.services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class Calculator
        : ICalculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
        public int Subtract(int x, int y)
        {
            return x - y;
        }

        public string PrintTypeName(Person person)
        {
            return person.GetType().Name;
        }
    }

    [ServiceContract]
    public interface ICalculator
    {
        [OperationContract]
        int Add(int x, int y);
        [OperationContract]
        int Subtract(int x, int y);
        [OperationContract]
        string PrintTypeName(Person person);
    }
}
