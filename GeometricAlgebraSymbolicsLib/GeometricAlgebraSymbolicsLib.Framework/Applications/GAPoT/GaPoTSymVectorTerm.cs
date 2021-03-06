﻿using System;
using System.Diagnostics;
using CodeComposerLib.LaTeX;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica.ExprFactory;
using Wolfram.NETLink;

namespace GeometricAlgebraSymbolicsLib.Applications.GAPoT
{
    public sealed class GaPoTSymVectorTerm
    {
        public static GaPoTSymVectorTerm operator -(GaPoTSymVectorTerm t)
        {
            return new GaPoTSymVectorTerm(
                t.TermId, 
                Mfs.Minus[t.Value].GaPoTSymSimplify()
            );
        }

        public static GaPoTSymVectorTerm operator +(GaPoTSymVectorTerm t1, GaPoTSymVectorTerm t2)
        {
            if (t1.TermId != t2.TermId)
                throw new InvalidOperationException();

            return new GaPoTSymVectorTerm(
                t1.TermId, 
                Mfs.Plus[t1.Value, t2.Value].GaPoTSymSimplify()
            );
        }

        public static GaPoTSymVectorTerm operator -(GaPoTSymVectorTerm t1, GaPoTSymVectorTerm t2)
        {
            if (t1.TermId != t2.TermId)
                throw new InvalidOperationException();

            return new GaPoTSymVectorTerm(
                t1.TermId, 
                Mfs.Subtract[t1.Value, t2.Value].GaPoTSymSimplify()
            );
        }

        public static GaPoTSymVectorTerm operator *(GaPoTSymVectorTerm t, Expr s)
        {
            return new GaPoTSymVectorTerm(
                t.TermId, 
                Mfs.Times[t.Value, s].GaPoTSymSimplify()
            );
        }

        public static GaPoTSymVectorTerm operator *(Expr s, GaPoTSymVectorTerm t)
        {
            return new GaPoTSymVectorTerm(
                t.TermId, 
                Mfs.Times[s, t.Value].GaPoTSymSimplify()
            );
        }

        public static GaPoTSymVectorTerm operator /(GaPoTSymVectorTerm t, Expr s)
        {
            return new GaPoTSymVectorTerm(
                t.TermId, 
                Mfs.Divide[t.Value, s].GaPoTSymSimplify()
            );
        }


        public int TermId { get; }

        public Expr Value { get; set; }


        internal GaPoTSymVectorTerm(int id)
        {
            Debug.Assert(id > 0);

            TermId = id;
            Value = Expr.INT_ZERO;
        }

        internal GaPoTSymVectorTerm(int id, Expr value)
        {
            Debug.Assert(id > 0);

            TermId = id;
            Value = value;
        }


        public bool IsZero()
        {
            return Value.IsZero();
        }

        public Expr Norm()
        {
            return Mfs.Abs[Value].GaPoTSymSimplify();
        }

        public Expr Norm2()
        {
            return Mfs.Times[Value, Value].GaPoTSymSimplify();
        }

        public GaPoTSymMultivectorTerm ToMultivectorTerm()
        {
            return new GaPoTSymMultivectorTerm(1 << (TermId - 1) , Value);
        }

        public string ToText()
        {
            // if (Value.IsZero())
            //     return "0";

            return $"'{Value}' <{TermId}>";
        }

        public string ToLaTeX()
        {
            // if (Value.IsZero())
            //     return "0";

            var valueText = Value.GetLaTeXScalar().LaTeXMathAddParentheses();
            var basisText = TermId.ToString().GetLaTeXBasisName();

            return $@"{valueText} {basisText}";
        }
 
        public GaPoTSymVectorTerm OffsetTermId(int delta)
        {
            var id = TermId + delta;

            return new GaPoTSymVectorTerm(id, Value);
        }

        public override string ToString()
        {
            return ToText();
        }
    }
}