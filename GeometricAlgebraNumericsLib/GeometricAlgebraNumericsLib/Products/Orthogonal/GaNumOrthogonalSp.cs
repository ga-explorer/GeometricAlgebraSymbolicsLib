﻿using GeometricAlgebraNumericsLib.Exceptions;
using GeometricAlgebraNumericsLib.Maps.Bilinear;
using GeometricAlgebraNumericsLib.Metrics;
using GeometricAlgebraNumericsLib.Multivectors.Numeric;
using GeometricAlgebraNumericsLib.Multivectors.Numeric.Factories;
using GeometricAlgebraStructuresLib.Frames;

namespace GeometricAlgebraNumericsLib.Products.Orthogonal
{
    public sealed class GaNumOrthogonalSp : GaNumBilinearProductOrthogonal
    {
        public override GaNumSarMultivector this[GaNumSarMultivector mv1, GaNumSarMultivector mv2]
        {
            get
            {
                if (mv1.GaSpaceDimension != DomainGaSpaceDimension || mv2.GaSpaceDimension != DomainGaSpaceDimension2)
                    throw new GaNumericsException("Multivector size mismatch");

                return mv1.GetGbtSpTerms(mv2, OrthogonalMetric).SumAsSarMultivector(TargetVSpaceDimension);
            }
        }

        public override GaNumDgrMultivector this[GaNumDgrMultivector mv1, GaNumDgrMultivector mv2]
        {
            get
            {
                if (mv1.GaSpaceDimension != DomainGaSpaceDimension || mv2.GaSpaceDimension != DomainGaSpaceDimension2)
                    throw new GaNumericsException("Multivector size mismatch");

                return mv1.GetGbtSpTerms(mv2, OrthogonalMetric).SumAsDgrMultivector(TargetVSpaceDimension);
            }
        }


        internal GaNumOrthogonalSp(GaNumMetricOrthogonal basisBladesSignatures)
            : base(basisBladesSignatures, GaNumMapBilinearAssociativity.NoneAssociative)
        {
        }


        public override GaNumTerm MapToTerm(int id1, int id2)
        {
            return GaNumTerm.Create(
                TargetGaSpaceDimension,
                0,
                GaFrameUtils.IsNonZeroESp(id1, id2)
                    ? (GaFrameUtils.IsNegativeEGp(id1, id1)
                        ? -OrthogonalMetric[id1]
                        : OrthogonalMetric[id1])
                    : 0.0d
            );
        }
    }
}