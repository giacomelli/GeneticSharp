using System;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// The default geometric converter leverages standard .Net converters to convert gene values to a back from metric space from gene space.
    /// </summary>
    /// <typeparam name="TGeneValue"></typeparam>
    public class DefaultGeometricConverter<TGeneValue> : IGeometricConverter<TGeneValue>
    {
        private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(TGeneValue));

        public double GeneToDouble(int geneIndex, TGeneValue geneValue) =>
            Convert.ToDouble(geneValue);


        public TGeneValue DoubleToGene(int geneIndex, double metricValue) =>
            (TGeneValue) _converter.ConvertFrom(metricValue);

    }
}