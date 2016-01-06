using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicScripting.Dynamics
{
    public interface IMyInterface
    {
        int Size { get; }

        double Compute(double factor);
    }

    public class MyInterfaceImplementorA
        : IMyInterface
    {
        public int Size
        {
            get { return 11; }
        }

        public double Compute(double factor)
        {
            return this.Size * factor;
        }
    }

    public class MyInterfaceImplementorB
        : IMyInterface
    {
        public int Size
        {
            get { return 7; }
        }

        public double Compute(double factor)
        {
            return this.Size * factor / 2;
        }
    }
}
