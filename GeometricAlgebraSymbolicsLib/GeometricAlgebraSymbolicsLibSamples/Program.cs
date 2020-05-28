using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometricAlgebraSymbolicsLibSamples.GAPoT;

namespace GeometricAlgebraSymbolicsLibSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            ParsingSample3.Execute();

            Console.WriteLine();
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadKey();
        }
    }
}
