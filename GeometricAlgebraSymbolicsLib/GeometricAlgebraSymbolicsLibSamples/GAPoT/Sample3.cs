﻿using System;
using System.Globalization;
using GeometricAlgebraSymbolicsLib.Applications.GAPoT;

namespace GeometricAlgebraSymbolicsLibSamples.GAPoT
{
    public static class Sample3
    {
        public static void Execute()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var mvU = 
                "p('233.92', '-1.57') <1,2>, p('0.46', '-2.61') <3,4>, p('4.74', '1.28') <5,6>, p('4.02', '-0.07') <7,8>, p('0.42', '-2.60') <9,10>"
                    .GaPoTSymParseVector();

            var mvI = 
                "p('2.33', '-0.72') <1,2>, p('0.93', '1.85') <3,4>, p('0.45', '-1.69') <5,6>, p('0.49', '1.70') <7,8>, p('0.16', '-1.44') <9,10>"
                    .GaPoTSymParseVector();

            var mvM = mvU * mvI;

            Console.WriteLine(@"Display multivector terms in text form");
            Console.WriteLine($@"U = {mvU}");
            Console.WriteLine($@"I = {mvI}");
            Console.WriteLine($@"M = {mvM}");
            Console.WriteLine();

            Console.WriteLine(@"Display multivector terms in LaTeX form");
            Console.WriteLine($@"U = {mvU.ToLaTeX()}");
            Console.WriteLine($@"I = {mvI.ToLaTeX()}");
            Console.WriteLine($@"M = {mvM.ToLaTeX()}");
            Console.WriteLine();

            Console.WriteLine(@"Display multivector polar phasors in text form");
            Console.WriteLine($@"U = {mvU.PolarPhasorsToText()}");
            Console.WriteLine($@"I = {mvI.PolarPhasorsToText()}");
            Console.WriteLine();

            Console.WriteLine(@"Display multivector polar phasors in LaTeX form");
            Console.WriteLine($@"U = {mvU.PolarPhasorsToLaTeX()}");
            Console.WriteLine($@"I = {mvI.PolarPhasorsToLaTeX()}");
            Console.WriteLine();
        }
    }
}