using System;
using System.Collections.Generic;
using System.Linq;
using DataStructuresLib.Dictionary;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica.ExprFactory;
using TextComposerLib.Text;
using Wolfram.NETLink;

namespace GeometricAlgebraSymbolicsLib.Applications.GAPoT
{
    public sealed class GaPoTSymBivector
    {
        public static GaPoTSymBivector operator -(GaPoTSymBivector v)
        {
            return new GaPoTSymBivector(
                v._termsDictionary.Values.Select(t => -t)
            );
        }

        public static GaPoTSymBivector operator +(GaPoTSymBivector v1, GaPoTSymBivector v2)
        {
            var biVector = new GaPoTSymBivector();

            biVector.AddTerms(v1._termsDictionary.Values);
            biVector.AddTerms(v2._termsDictionary.Values);

            return biVector;
        }

        public static GaPoTSymBivector operator -(GaPoTSymBivector v1, GaPoTSymBivector v2)
        {
            var biVector = new GaPoTSymBivector();

            biVector.AddTerms(v1._termsDictionary.Values);
            biVector.AddTerms(v2._termsDictionary.Values.Select(t => -t));

            return biVector;
        }


        private readonly Dictionary2Keys<int, GaPoTSymBivectorTerm> _termsDictionary
            = new Dictionary2Keys<int, GaPoTSymBivectorTerm>();


        public GaPoTSymBivector()
        {
        }

        public GaPoTSymBivector(IEnumerable<GaPoTSymBivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);
        }


        public GaPoTSymBivector AddTerm(GaPoTSymBivectorTerm term)
        {
            if (_termsDictionary.TryGetValue(term.TermId1, term.TermId2, out var oldTerm))
                _termsDictionary[term.TermId1, term.TermId2] = new GaPoTSymBivectorTerm(
                    term.TermId1,
                    term.TermId2,
                    Mfs.Plus[oldTerm.Value, term.Value].GaPoTSymSimplify()
                );
            else
                _termsDictionary.Add(term.TermId1, term.TermId2, term);

            return this;
        }

        public GaPoTSymBivector AddTerm(int id1, int id2, Expr value)
        {
            return AddTerm(
                new GaPoTSymBivectorTerm(id1, id2, value)
            );
        }

        public GaPoTSymBivector AddTerms(IEnumerable<GaPoTSymBivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);

            return this;
        }


        public IEnumerable<GaPoTSymBivectorTerm> GetTerms()
        {
            return _termsDictionary.Values.Where(t => !t.Value.IsZero());
        }

        public Expr GetTermValuesSum()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Select(v => v.Value)
                .ToArray()
            ).GaPoTSymSimplify();
        }


        public Expr GetActiveTotal()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Where(t => t.IsScalar)
                .Select(v => v.Value)
                .ToArray()
            ).GaPoTSymSimplify();
        }

        public Expr GetReactiveTotal()
        {
            return Mfs.SumExpr(
                _termsDictionary
                    .Values
                    .Where(t => t.IsPhasor)
                    .Select(v => v.Value)
                    .ToArray()
            ).GaPoTSymSimplify();
        }

        public Expr GetNonActiveTotal()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Where(t => t.IsNonScalar)
                .Select(v => v.Value)
                .ToArray()
            ).GaPoTSymSimplify();
        }

        public Expr GetReactiveFundamentalTotal()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Where(t => t.TermId1 == 1 && t.TermId2 == 2)
                .Select(v => v.Value)
                .ToArray()
            ).GaPoTSymSimplify();
        }

        public Expr GetHarmTotal()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Where(t => t.IsNonScalar && (t.TermId1 != 1 || t.TermId2 != 2))
                .Select(v => v.Value)
                .ToArray()
            ).GaPoTSymSimplify();
        }


        public GaPoTSymBivector GetActivePart()
        {
            return new GaPoTSymBivector(
                _termsDictionary.Values.Where(t => t.IsScalar)
            );
        }

        public GaPoTSymBivector GetReactivePart()
        {
            return new GaPoTSymBivector(
                _termsDictionary.Values.Where(t => t.IsPhasor)
            );
        }

        public GaPoTSymBivector GetNonActivePart()
        {
            return new GaPoTSymBivector(
                _termsDictionary.Values.Where(t => t.IsNonScalar)
            );
        }

        public GaPoTSymBivector GetReactiveFundamentalPart()
        {
            return new GaPoTSymBivector(
                _termsDictionary.Values.Where(t => t.TermId1 == 1 && t.TermId2 == 2)
            );
        }

        public GaPoTSymBivector GetHarmPart()
        {
            return new GaPoTSymBivector(
                _termsDictionary.Values.Where(t => t.IsNonScalar && (t.TermId1 != 1 || t.TermId2 != 2))
            );
        }


        public GaPoTSymBivector Reverse()
        {
            var result = new GaPoTSymBivector();

            foreach (var pair in _termsDictionary)
            {
                if (pair.Value.IsScalar)
                    result.AddTerm(pair.Value);
                else
                    result.AddTerm(-pair.Value);
            }

            return result;
        }

        public GaPoTSymBivector NegativeReverse()
        {
            var result = new GaPoTSymBivector();

            foreach (var pair in _termsDictionary)
            {
                if (pair.Value.IsScalar)
                    result.AddTerm(-pair.Value);
                else
                    result.AddTerm(pair.Value);
            }

            return result;
        }

        public Expr Norm()
        {
            var termsArray = 
                _termsDictionary
                    .Values
                    .Select(t => Mfs.Times[t.Value, t.Value])
                    .ToArray();

            return Mfs.Sqrt[Mfs.SumExpr(termsArray)].GaPoTSymSimplify();
        }

        public Expr Norm2()
        {
            var termsArray = 
                _termsDictionary
                    .Values
                    .Select(t => Mfs.Times[t.Value, t.Value])
                    .ToArray();

            return Mfs.SumExpr(termsArray).GaPoTSymSimplify();
        }

        public GaPoTSymBivector Inverse()
        {
            var norm2 = Norm2();

            if (norm2.IsZero())
                throw new DivideByZeroException();

            var value = Mfs.Divide[1, norm2];

            return new GaPoTSymBivector(
                _termsDictionary
                    .Values
                    .Select(t => t.ScaledReverse(value))
            );
        }

        public GaPoTSymVector Gp(GaPoTSymVector v)
        {
            return this * v;
        }


        public string ToText()
        {
            return TermsToText();
        }

        public string TermsToText()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }


        public string ToLaTeX()
        {
            return TermsToLaTeX();
        }

        public string TermsToLaTeX()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }
 

        public override string ToString()
        {
            return TermsToText();
        }
    }
}