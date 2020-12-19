using System;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// The standard geometric converter allows defining chromosome specific Gene values converters 
    /// </summary>
    /// <typeparam name="TGeneValue"></typeparam>
    public class GeometricConverter<TGeneValue> : IGeometricConverter<TGeneValue>
    {
        public static readonly TypeConverter DefaultTypeConverter = TypeDescriptor.GetConverter(typeof(TGeneValue));

        public Func<int, TGeneValue, double> GeneToDoubleConverter { get; set; }

        public Func<int, double, TGeneValue> DoubleToGeneConverter { get; set; }


        public double GeneToDouble(int geneIndex, TGeneValue geneValue) => GeneToDoubleConverter(geneIndex, geneValue);


        public TGeneValue DoubleToGene(int geneIndex, double metricValue) => DoubleToGeneConverter(geneIndex, metricValue);

    }
}