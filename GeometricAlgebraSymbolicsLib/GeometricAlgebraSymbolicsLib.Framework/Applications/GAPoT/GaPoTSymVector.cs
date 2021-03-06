﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static GaPoTSymBiversor operator *(GaPoTSymVector v1, GaPoTSymVector v2)
        {
            var bivector = new GaPoTSymBiversor();

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

        public static GaPoTSymVector operator *(GaPoTSymBiversor bv1, GaPoTSymVector v2)
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

        public static GaPoTSymVector operator *(GaPoTSymVector v1, GaPoTSymBiversor bv2)
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
                    Mfs.Times[term.Value, s].GaPoTSymSimplify()
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

        public static GaPoTSymVector operator /(GaPoTSymVector v, Expr s)
        {
            var result = new GaPoTSymVector();

            foreach (var term in v._termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    Mfs.Divide[term.Value, s].GaPoTSymSimplify()
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
            Debug.Assert(id > 0);
            
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
            return SetPolarPhasor(
                phasor.Id, 
                phasor.Magnitude, 
                phasor.Phase
            );
        }

        public GaPoTSymVector SetRectPhasor(int id, Expr x, Expr y)
        {
            Debug.Assert(id > 0 && id % 2 == 1);
            
            SetTerm(id, x);

            SetTerm(id + 1, Mfs.Minus[y].GaPoTSymSimplify());

            return this;
        }


        public GaPoTSymVector AddTerm(int id, Expr value)
        {
            Debug.Assert(id > 0);
            
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
        

        public GaPoTSymVectorTerm GetTerm(int id)
        {
            Debug.Assert(id > 0);
            
            if (_termsDictionary.TryGetValue(id, out var term))
                return new GaPoTSymVectorTerm(term.TermId, term.Value);

            return new GaPoTSymVectorTerm(id);
        }

        public GaPoTSymPolarPhasor GetPolarPhasor(int id)
        {
            return GetRectPhasor(id).ToPolarPhasor();
        }

        public GaPoTSymRectPhasor GetRectPhasor(int id)
        {
            Debug.Assert(id > 0 && id % 2 == 1);
            
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
        
        public GaPoTSymVector GetOffsetPartByTermIDsRange(int minTermId, int maxTermId)
        {
            var termsList = 
                GetTerms()
                    .Where(t => t.TermId >= minTermId && t.TermId <= maxTermId)
                    .Select(t => t.OffsetTermId(1 - minTermId));

            return new GaPoTSymVector(termsList);
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

        public GaPoTSymVector[] GetOffsetParts(params int[] partLengthsArray)
        {
            var results = new GaPoTSymVector[partLengthsArray.Length];

            var termId1 = 1;
            for (var i = 0; i < partLengthsArray.Length; i++)
            {
                var termId2 = termId1 + partLengthsArray[i] - 1;

                results[i] = GetOffsetPartByTermIDsRange(termId1, termId2);

                termId1 = termId2 + 1;
            }

            return results;
        }

        public GaPoTSymBiversor[] GetPartsImpedance(GaPoTSymVector current, params int[] partLengthsArray)
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
            var phasorsDict = new Dictionary<int, GaPoTSymRectPhasor>();

            foreach (var term in _termsDictionary.Values)
            {
                var id = term.TermId;
                var x = Expr.INT_ZERO;
                var y = Expr.INT_ZERO;

                if (id % 2 == 1)
                {
                    x = term.Value;
                }
                else
                {
                    id--;
                    y = Mfs.Minus[term.Value].GaPoTSymSimplify();
                }

                if (phasorsDict.TryGetValue(id, out var phasor))
                {
                    phasor.XValue = Mfs.Plus[phasor.XValue, x].GaPoTSymSimplify();
                    phasor.YValue = Mfs.Plus[phasor.YValue, y].GaPoTSymSimplify();
                }
                else
                {
                    phasorsDict.Add(id, new GaPoTSymRectPhasor(id, x, y));
                }
            }

            return phasorsDict.Values;
        }
        

        public Expr DotProduct(GaPoTSymVector v)
        {
            var resultList = new List<Expr>(_termsDictionary.Count);

            foreach (var term1 in _termsDictionary.Values)
            {
                if (v._termsDictionary.TryGetValue(term1.TermId, out var term2)) 
                    resultList.Add(
                        Mfs.Times[term1.Value, term2.Value]
                    );
            }

            return Mfs.SumExpr(resultList.ToArray()).GaPoTSymSimplify();
        }

        public Expr GetAngle(GaPoTSymVector v)
        {
            return Mfs.ArcCos[
                Mfs.Divide[
                    DotProduct(v), 
                    Mfs.Sqrt[Mfs.Times[Norm2(), v.Norm2()]]
                ]
            ].GaPoTSymSimplify();
        }

        public GaPoTSymMultivector Op(GaPoTSymVector v)
        {
            return ToMultivector().Op(v.ToMultivector());
        }
        
        public GaPoTSymMultivector Op(GaPoTSymMultivector v)
        {
            return ToMultivector().Op(v);
        }
        
        public GaPoTSymMultivector Gp(GaPoTSymMultivector v)
        {
            return ToMultivector().Gp(v);
        }
        
        public GaPoTSymBiversor Gp(GaPoTSymVector v)
        {
            return this * v;
        }

        public GaPoTSymVector Gp(GaPoTSymBiversor bv)
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

        public GaPoTSymMultivector GetRotorToVector(GaPoTSymVector v2)
        {
            var cosAngle = Mfs.Divide[
                DotProduct(v2), 
                Mfs.Sqrt[Mfs.Times[Norm2(), v2.Norm2()]]
            ].GaPoTSymSimplify();

            if (cosAngle.IsOne())
                return new GaPoTSymMultivector(new []{ new GaPoTSymMultivectorTerm(0, Expr.INT_ONE) });
            
            //TODO: Handle the case for cosAngle == -1
            
            var cosHalfAngle = 
                Mfs.Sqrt[Mfs.Divide[Mfs.Plus[Expr.INT_ONE, cosAngle], 2.ToExpr()]];
            
            var sinHalfAngle = 
                Mfs.Sqrt[Mfs.Divide[Mfs.Subtract[Expr.INT_ONE, cosAngle], 2.ToExpr()]];
            
            var rotationBlade = Op(v2);

            var rotationBladeScalar =
                Mfs.Divide[
                    sinHalfAngle, 
                    Mfs.Sqrt[Mfs.Abs[rotationBlade.Gp(rotationBlade).GetTermValue(0)]]
                ];

            var rotor=  
                cosHalfAngle - rotationBladeScalar * rotationBlade;
            
            //var rotationAngle = Math.Acos(DotProduct(v2) * invNorm1 * invNorm2) / 2;
            //var unitBlade = rotationBlade.ScaleBy(rotationBladeInvNorm);
            //var unitBladeNorm = unitBlade.Gp(unitBlade).TermsToText();
            //var rotor= Math.Cos(rotationAngle) - (rotationBladeInvNorm * Math.Sin(rotationAngle)) * rotationBlade;

            //Normalize rotor
            //var invRotorNorm = 1.0d / Math.Sqrt(rotor.Gp(rotor.Reverse()).GetTermValue(0));
            
            return rotor;
        }
        
        public GaPoTSymVector ApplyRotor(GaPoTSymMultivector rotor)
        {
            var r1 = rotor;
            var r2 = rotor.Reverse();
            var v = ToMultivector();

            var mv = r1.Gp(v).Gp(r2);

            return mv.GetVectorPart();
        }

        public GaPoTSymVector ApplyRotor(GaPoTSymBiversor rotor)
        {
            var r1 = rotor.ToMultivector();
            var r2 = rotor.Reverse().ToMultivector();
            var v = ToMultivector();

            var mv = r1.Gp(v).Gp(r2);

            return mv.GetVectorPart();
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

        public GaPoTSymVector OffsetTermIDs(int delta)
        {
            return new GaPoTSymVector(
                _termsDictionary
                    .Values
                    .Select(t => t.OffsetTermId(delta))
            );
        }


        public string ToText()
        {
            return PolarPhasorsToText();
        }

        public string TermsToText()
        {
            var termsArray = 
                GetTerms().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string PolarPhasorsToText()
        {
            var termsArray = 
                GeTPolarPhasors().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string RectPhasorsToText()
        {
            var termsArray = 
                GeTRectPhasors().Where(t => !t.IsZero()).ToArray();

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
                GetTerms().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string PolarPhasorsToLaTeX()
        {
            var termsArray = 
                GeTPolarPhasors().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string RectPhasorsToLaTeX()
        {
            var termsArray = 
                GeTRectPhasors().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }


        public GaPoTSymMultivector ToMultivector()
        {
            return new GaPoTSymMultivector(
                GetTerms().Select(t => t.ToMultivectorTerm())
            );
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