using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeComposerLib.Irony;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica.ExprFactory;
using Irony.Parsing;
using Wolfram.NETLink;

namespace GeometricAlgebraSymbolicsLib.Applications.GAPoT
{
    public static class GaPoTSymUtils
    {
        public static Expr GaPoTSymToScalar(this string exprText)
        {
            if (exprText[0] == '\'' && exprText[exprText.Length - 1] == '\'')
                exprText = exprText
                    .Substring(1, exprText.Length - 2)
                    .Trim();

            var expr = exprText.ToExpr(MathematicaInterface.DefaultCas);

            return expr;
        }

        public static Expr GaPoTSymSimplify(this Expr expr)
        {
            return expr.Simplify(MathematicaInterface.DefaultCas);
        }

        public static Expr GaPoTSymExpand(this Expr expr)
        {
            return expr.Expand(MathematicaInterface.DefaultCas);
        }

        public static string GaPoTSymEvaluateToString(this Expr expr)
        {
            return MathematicaInterface.DefaultCasConnection.EvaluateToString(expr);
        }

        public static string GetLaTeXScalar(this Expr expr)
        {
            return Mfs.EToString[Mfs.TeXForm[expr]].GaPoTSymEvaluateToString();
        }


        private static GaPoTSymVector GaPoTSymParseVector(IronyParsingResults parsingResults, ParseTreeNode rootNode)
        {
            if (rootNode.ToString() != "spVector")
                throw new SyntaxErrorException(parsingResults.ToString());

            var vector = new GaPoTSymVector();

            var vectorNode = rootNode;
            foreach (var vectorElementNode in vectorNode.ChildNodes)
            {
                if (vectorElementNode.ToString() == "spTerm")
                {
                    //Term Form
                    var value = vectorElementNode.ChildNodes[0].FindTokenAndGetText().GaPoTSymToScalar();
                    var id = int.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());

                    if (id < 0)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddTerm(id, value);
                }
                else if (vectorElementNode.ToString() == "spPolarPhasor")
                {
                    //Polar Phasor Form
                    var magnitude = vectorElementNode.ChildNodes[1].FindTokenAndGetText().GaPoTSymToScalar();
                    var phase = vectorElementNode.ChildNodes[2].FindTokenAndGetText().GaPoTSymToScalar();
                    var id1 = int.Parse(vectorElementNode.ChildNodes[3].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[4].FindTokenAndGetText());

                    if (id1 < 0 || id2 != id1 + 1)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddPolarPhasor(id1, magnitude, phase);
                }
                else if (vectorElementNode.ToString() == "spRectPhasor")
                {
                    //Rectangular Phasor Form
                    var xValue = vectorElementNode.ChildNodes[1].FindTokenAndGetText().GaPoTSymToScalar();
                    var yValue = vectorElementNode.ChildNodes[2].FindTokenAndGetText().GaPoTSymToScalar();
                    var id1 = int.Parse(vectorElementNode.ChildNodes[3].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[4].FindTokenAndGetText());

                    if (id1 < 0 || id2 != id1 + 1)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddRectPhasor(id1, xValue, yValue);
                }
                else
                {
                    throw new SyntaxErrorException(parsingResults.ToString());
                }
            }

            return vector;
        }

        public static GaPoTSymVector GaPoTSymParseVector(this string sourceText)
        {
            var parsingResults = new IronyParsingResults(
                new GaPoTSymVectorConstructorGrammar(), 
                sourceText
            );

            if (parsingResults.ContainsErrorMessages || !parsingResults.ContainsParseTreeRoot)
                throw new SyntaxErrorException(parsingResults.ToString());

            return GaPoTSymParseVector(parsingResults, parsingResults.ParseTreeRoot);
        }


        private static GaPoTSymBivector GaPoTSymParseBivector(IronyParsingResults parsingResults, ParseTreeNode rootNode)
        {
            if (rootNode.ToString() != "bivector")
                throw new SyntaxErrorException(parsingResults.ToString());

            var vector = new GaPoTSymBivector();

            var vectorNode = rootNode;
            foreach (var vectorElementNode in vectorNode.ChildNodes)
            {
                if (vectorElementNode.ToString() == "bivectorTerm0")
                {
                    //Term Form
                    var value = vectorElementNode.ChildNodes[0].FindTokenAndGetText().GaPoTSymToScalar();

                    vector.AddTerm(1, 1, value);
                }
                else if (vectorElementNode.ToString() == "bivectorTerm2")
                {
                    //Term Form
                    var value = vectorElementNode.ChildNodes[0].FindTokenAndGetText().GaPoTSymToScalar();
                    var id1 = int.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[2].FindTokenAndGetText());

                    if (id1 < 0 || id2 < 0)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddTerm(id1, id2, value);
                }
                else
                {
                    throw new SyntaxErrorException(parsingResults.ToString());
                }
            }

            return vector;
        }

        public static GaPoTSymBivector GaPoTSymParseBivector(this string sourceText)
        {
            var parsingResults = new IronyParsingResults(
                new GaPoTSymBivectorConstructorGrammar(), 
                sourceText
            );

            if (parsingResults.ContainsErrorMessages || !parsingResults.ContainsParseTreeRoot)
                throw new SyntaxErrorException(parsingResults.ToString());

            return GaPoTSymParseBivector(parsingResults, parsingResults.ParseTreeRoot);
        }


        public static GaPoTSymVector[] Negative(this IEnumerable<GaPoTSymVector> vectorsList)
        {
            return vectorsList.Select(v => v.Negative()).ToArray();
        }

        public static GaPoTSymVector[] Inverse(this IEnumerable<GaPoTSymVector> vectorsList)
        {
            return vectorsList.Select(v => v.Inverse()).ToArray();
        }

        public static GaPoTSymVector[] Reverse(this IEnumerable<GaPoTSymVector> vectorsList)
        {
            return vectorsList.Select(v => v.Reverse()).ToArray();
        }

        public static Expr[] Norm(this IEnumerable<GaPoTSymVector> vectorsList)
        {
            return vectorsList.Select(v => v.Norm()).ToArray();
        }

        public static Expr[] Norm2(this IEnumerable<GaPoTSymVector> vectorsList)
        {
            return vectorsList.Select(v => v.Norm2()).ToArray();
        }

        public static GaPoTSymVector[] Add(this GaPoTSymVector[] vectorsList1, GaPoTSymVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTSymVector[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Add(vectorsList2[i]);

            return results;
        }

        public static GaPoTSymVector[] Subtract(this GaPoTSymVector[] vectorsList1, GaPoTSymVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTSymVector[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Subtract(vectorsList2[i]);

            return results;
        }

        public static GaPoTSymBivector[] Gp(this GaPoTSymVector[] vectorsList1, GaPoTSymVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTSymBivector[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Gp(vectorsList2[i]);

            return results;
        }
    }
}
