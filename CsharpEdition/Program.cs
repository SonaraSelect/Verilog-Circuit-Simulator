// // See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System;
using CircuitSimBackend;

namespace CircuitSimBackend
{
    class Program
    {
        static void Main(String[] args)
        {
            DataWrapper<int> intWrapper = new DataWrapper<int>(0);
            for (int i = 1; i < 10; i++)
            {
                intWrapper.Add(i);
            }
            intWrapper.Add(400);
            Console.WriteLine(intWrapper.ToString());
            Console.WriteLine(intWrapper.Count());

            Entity entity = new Entity("Charles", GateType.WIRE);
            
        }
    }
}
