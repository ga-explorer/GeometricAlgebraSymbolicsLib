using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica;
using GeometricAlgebraSymbolicsLib.Cas.Mathematica.ExprFactory;
using TextComposerLib.Text;
using Wolfram.NETLink;

namespace GeometricAlgebraSymbolicsLib.Applications.GAPoT
{
    public sealed class GaPoTSymVector : IReadOnlyList<Expr>
    {
        public static GaPoTSymVector operator -(GaPoTSymVector v)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    Mfs.Minus[term.Value].GaPoTSymSimplify()
                );

            return result;
        }

        public static GaPoTSymVector operator +(GaPoTSymVector v1, GaPoTSymVector v2)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v1._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value
                );

            return result;
        }

        public static GaPoTSymVector operator -(GaPoTSymVector v1, GaPoTSymVector v2)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v1._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    Mfs.Minus[term.Value].GaPoTSymSimplify()
                );

            return result;

        }

        /// <summary>
        /// The geometric product of two single phase GAPoT vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static GaPoTSymBivector operator *(GaPoTSymVector v1, GaPoTSymVector v2)
        {
            var bivector = new GaPoTSymBivector();

            foreach (var term1 in v1.GetTerms())
            {
                foreach (var term2 in v2.GetTerms())
                {
                    var scalarValue = 
                        Mfs.Times[term1.Value, term2.Value].GaPoTSymSimplify();

                    bivector.AddTerm(
                        term1.TermId, 
                        term2.TermId, 
                        scalarValue
                    );
                }
            }

            return bivector;
        }

        public static GaPoTSymVector operator *(GaPoTSymBivector bv1, GaPoTSymVector v2)
        {
            var vector = new GaPoTSymVector();

            foreach (var term1 in bv1.GetTerms())
            {
                foreach (var term2 in v2.GetTerms())
                {
                    var scalarValue = Mfs.Times[term1.Value, term2.Value];

                    if (term1.IsScalar)
                    {
                        vector.AddTerm(
                            term2.TermId, 
                            scalarValue
                        );

                        continue;
                    }

                    if (term1.TermId1 == term2.TermId)
                    {
                        vector.AddTerm(
                            term1.TermId2, 
                            Mfs.Minus[scalarValue]
                        );

                        continue;
                    }
                    
                    if (term1.TermId2 == term2.TermId)
                    {
                        vector.AddTerm(
                            term1.TermId1, 
                            scalarValue
                        );
                    }
                }
            }

            return vector;
        }

        public static GaPoTSymVector operator *(GaPoTSymVector v1, GaPoTSymBivector bv2)
        {
            var vector = new GaPoTSymVector();

            foreach (var term1 in v1.GetTerms())
            {
                foreach (var term2 in bv2.GetTerms())
                {
                    var scalarValue = Mfs.Times[term1.Value, term2.Value];

                    if (term2.IsScalar)
                    {
                        vector.AddTerm(
                            term1.TermId, 
                            scalarValue
                        );

                        continue;
                    }

                    if (term1.TermId == term2.TermId1)
                    {
                        vector.AddTerm(
                            term2.TermId2, 
                            scalarValue
                        );

                        continue;
                    }
                    
                    if (term1.TermId == term2.TermId2)
                    {
                        vector.AddTerm(
                            term2.TermId1, 
                            Mfs.Minus[scalarValue]
                        );
                    }
                }
            }

            return vector;
        }

        public static GaPoTSymVector operator *(GaPoTSymVector v, Expr s)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    Mfs.Times[s, term.Value].GaPoTSymSimplify()
                );

            return result;
        }

        public static GaPoTSymVector operator *(Expr s, GaPoTSymVector v)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    Mfs.Times[s, term.Value].GaPoTSymSimplify()
                );

            return result;
        }


        private readonly SortedDictionary<int, GaPoTSymVectorTerm> _termsDictionary
            = new SortedDictionary<int, GaPoTSymVectorTerm>();


        public int Count 
            => _termsDictionary.Count;

        public Expr this[int index] 
            => _termsDictionary.TryGetValue(index, out var value) 
                ? value.Value : Expr.INT_ZERO;


        public GaPoTSymVector()
        {
        }

        public GaPoTSymVector(IEnumerable<GaPoTSymVectorTerm> termsList)
        {
            AddTerms(termsList);
        }


        public GaPoTSymVector SetToZero()
        {
            _termsDictionary.Clear();

            return this;
        }


        public bool IsZero()
        {
            return Norm2().IsZero();
        }


        public GaPoTSymVector SetTerm(int id, Expr value)
        {
            if (_termsDictionary.ContainsKey(id))
                _termsDictionary[id].Value = value;
            else
                _termsDictionary.Add(id, new GaPoTSymVectorTerm(id, value));

            return this;
        }

        public GaPoTSymVector SetPolarPhasor(int id, Expr magnitude, Expr phase)
        {
            return SetRectPhasor(
                id,
                Mfs.Times[magnitude, Mfs.Cos[phase]],
                Mfs.Times[magnitude, Mfs.Sin[phase]]
            );
        }

        public GaPoTSymVector SetPolarPhasor(GaPoTSymPolarPhasor phasor)
        {
            return SetPolarPhasor(phasor.Id, phasor.Magnitude, phasor.Phase);
        }

        public GaPoTSymVector SetRectPhasor(int id, Expr x, Expr y)
        {
            SetTerm(id, x);

            SetTerm(id + 1, Mfs.Minus[y].GaPoTSymSimplify());

            return this;
        }


        public GaPoTSymVector AddTerm(int id, Expr value)
        {
            if (_termsDictionary.ContainsKey(id))
                _termsDictionary[id].Value = Mfs.Plus[_termsDictionary[id].Value, value].GaPoTSymSimplify();
            else
                _termsDictionary.Add(id, new GaPoTSymVectorTerm(id, value));

            return this;
        }
        
        public GaPoTSymVector AddTerm(GaPoTSymVectorTerm term)
        {
            return AddTerm(term.TermId, term.Value);
        }
        
        public GaPoTSymVector AddTerms(IEnumerable<GaPoTSymVectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term.TermId, term.Value);

            return this;
        }
        
        public GaPoTSymVector AddPolarPhasor(int id, Expr magnitude, Expr phase)
        {
            return AddRectPhasor(
                id,
                Mfs.Times[magnitude, Mfs.Cos[phase]],
                Mfs.Times[magnitude, Mfs.Sin[phase]]
            );
        }

        public GaPoTSymVector AddPolarPhasor(GaPoTSymPolarPhasor phasor)
        {
            return AddPolarPhasor(phasor.Id, phasor.Magnitude, phasor.Phase);
        }

        public GaPoTSymVector AddRectPhasor(int id, Expr x, Expr y)
        {
            AddTerm(id, x);

            AddTerm(id + 1, Mfs.Minus[y].GaPoTSymSimplify());

            return this;
        }
        

        public GaPoTSymPolarPhasor GetPolarPhasor(int id)
        {
            return GetRectPhasor(id).ToPolarPhasor();
        }

        public GaPoTSymRectPhasor GetRectPhasor(int id)
        {
            var x = this[id];
            var y = Mfs.Minus[this[id + 1]].GaPoTSymSimplify();

            return new GaPoTSymRectPhasor(id, x, y);
        }

        public GaPoTSymVector GetPartByTermIDs(params int[] termIDsList)
        {
            return new GaPoTSymVector(
                GetTerms().Where(t => termIDsList.Contains(t.TermId))
            );
        }
        
        public GaPoTSymVector GetPartByTermIDsRange(int minTermId, int maxTermId)
        {
            return new GaPoTSymVector(
                GetTerms().Where(t => t.TermId >= minTermId && t.TermId <= maxTermId)
            );
        }

        public GaPoTSymVector[] GetParts(params int[] partLengthArray)
        {
            var results = new GaPoTSymVector[partLengthArray.Length];

            var termId1 = 1;
            for (var i = 0; i < partLengthArray.Length; i++)
            {
                var termId2 = termId1 + partLengthArray[i] - 1;

                results[i] = GetPartByTermIDsRange(termId1, termId2);

                termId1 = termId2 + 1;
            }

            return results;
        }

        public GaPoTSymBivector[] GetPartsImpedance(GaPoTSymVector current, params int[] partLengthsArray)
        {
            var mvU = GetParts(partLengthsArray);
            var mvI = current.GetParts(partLengthsArray).Inverse();

            return mvU.Gp(mvI);
        }

        public IEnumerable<GaPoTSymVectorTerm> GetTerms()
        {
            return _termsDictionary.Values;
        }

        public IEnumerable<GaPoTSymPolarPhasor> GeTPolarPhasors()
        {
            return GeTRectPhasors().Select(p => p.ToPolarPhasor());
        }

        public IEnumerable<GaPoTSymRectPhasor> GeTRectPhasors()
        {
            var termIDsList = _termsDictionary
                .Keys
                .Where(id => id % 2 == 1);

            foreach (var id in termIDsList)
            {
                var x = this[id];
                var y = Mfs.Minus[this[id + 1]].GaPoTSymSimplify();

                yield return new GaPoTSymRectPhasor(id, x, y);
            }
        }
        

        public GaPoTSymBivector Gp(GaPoTSymVector v)
        {
            return this * v;
        }

        public GaPoTSymVector Gp(GaPoTSymBivector bv)
        {
            return this * bv;
        }

        public GaPoTSymVector Add(GaPoTSymVector v)
        {
            return this + v;
        }

        public GaPoTSymVector Subtract(GaPoTSymVector v)
        {
            return this - v;
        }

        public GaPoTSymVector Negative()
        {
            return -this;
        }

        public GaPoTSymVector Reverse()
        {
            return this;
        }

        public Expr Norm()
        {
            return Mfs.Sqrt[Mfs.SumExpr(
            _termsDictionary
                .Values
                .Select(p => p.Norm2())
                .ToArray()
            )].GaPoTSymSimplify();
        }

        public Expr Norm2()
        {
            return Mfs.SumExpr(
                _termsDictionary
                .Values
                .Select(p => p.Norm2())
                .ToArray()
            ).GaPoTSymSimplify();
        }

        public GaPoTSymVector Inverse()
        {
            var norm2 = Norm2();

            var result = new GaPoTSymVector();

            if (norm2.IsZero())
                throw new DivideByZeroException();

            foreach (var term in _termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    Mfs.Divide[term.Value, norm2].GaPoTSymSimplify()
                );

            return result;
        }


        public string ToText()
        {
            return PolarPhasorsToText();
        }

        public string TermsToText()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string PolarPhasorsToText()
        {
            var termsArray = 
                GeTPolarPhasors().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string RectPhasorsToText()
        {
            var termsArray = 
                GeTRectPhasors().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }


        public string ToLaTeX()
        {
            return PolarPhasorsToLaTeX();
        }

        public string TermsToLaTeX()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string PolarPhasorsToLaTeX()
        {
            var termsArray = 
                GeTPolarPhasors().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string RectPhasorsToLaTeX()
        {
            var termsArray = 
                GeTRectPhasors().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }


        public IEnumerator<Expr> GetEnumerator()
        {
            return _termsDictionary
                .Values
                .Select(v => v.Value)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _termsDictionary
                .Values
                .Select(v => v.Value)
                .GetEnumerator();
        }

        public override string ToString()
        {
            return ToText();
        }
    }
}