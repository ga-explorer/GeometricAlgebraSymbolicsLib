using System;
using GeometricAlgebraSymbolicsLibSamples.GAPoT;

namespace GeometricAlgebraSymbolicsLibSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            SequentialRotationSample.Execute();

            Console.WriteLine();
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadKey();
        }
    }
}
