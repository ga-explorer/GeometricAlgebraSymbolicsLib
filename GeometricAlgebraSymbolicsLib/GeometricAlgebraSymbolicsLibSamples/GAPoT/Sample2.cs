﻿using System;
using System.Globalization;
using GeometricAlgebraSymbolicsLib.Applications.GAPoT;

namespace GeometricAlgebraSymbolicsLibSamples.GAPoT
{
    public static class Sample2
    {
        public static void Execute()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            
            var mvU = "'1'<1>, '1'<2>, '1'<3>, '2'<4>, '1'<5>, '1'<7>, '-1'<8>"
                .GaPoTSymParseVector();

            var mvI = "['1'<1>, '1'<2>, '1'<3>, '3'<4>, '1/2'<5>, '1'<7>"
                .GaPoTSymParseVector();
           
            Console.WriteLine(@"Display multivectors in LaTeX form");
            Console.WriteLine($@"U = {mvU.ToLaTeX()}");
            Console.WriteLine($@"I = {mvI.ToLaTeX()}");
            Console.WriteLine();
           
            Console.WriteLine(@"Display multivectors in text form");
            Console.WriteLine($@"U = {mvU}");
            Console.WriteLine($@"I = {mvI}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display the inverse");
            Console.WriteLine($@"inv(U) = {mvU.Inverse().ToLaTeX()}");
            Console.WriteLine($@"inv(I) = {mvI.Inverse().ToLaTeX()}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display geometric product of multivectors U * inv(U)");
            Console.WriteLine($@"U * inv(U) = {(mvU * mvU.Inverse()).ToLaTeX()}");
            Console.WriteLine($@"I * inv(I) = {(mvI * mvI.Inverse()).ToLaTeX()}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display geometric product of multivectors");
            Console.WriteLine($@"U * I = {mvU * mvI}");
            Console.WriteLine($@"U * I = {(mvU * mvI).ToLaTeX()}");
            Console.WriteLine();
        }
    }
}