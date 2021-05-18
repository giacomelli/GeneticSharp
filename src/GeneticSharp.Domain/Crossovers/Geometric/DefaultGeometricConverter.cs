using System;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers.Geometric
{

    /// <summary>
    /// The default geometric converter leverages standard .Net converters to convert gene values to a back from metric space from gene space.
    /// </summary>
    /// <typeparam name="TGeneValue"></typeparam>
    public class DefaultGeometricConverter : IGeometricConverter
    {

        private readonly TypedGeometricConverter _typedConverter;

        public DefaultGeometricConverter()
        {
            var converter = new TypedGeometricConverter();
            converter.SetTypedConverter<object>(new DefaultGeometricConverter<object>());
            _typedConverter = converter;
        }

        public object DoubleToGene(int geneIndex, double metricValue)
        {
            return _typedConverter.DoubleToGene(geneIndex, metricValue);
        }

        IGeometryEmbedding<object> IGeometricConverter<object>.GetEmbedding()
        {
            return _typedConverter.GetEmbedding();
        }

        public bool IsOrdered
        {
            get => _typedConverter.IsOrdered;
            set => _typedConverter.IsOrdered = value;
        }

        public double GeneToDouble(int geneIndex, object geneValue)
        {
            return _typedConverter.GeneToDouble(geneIndex, geneValue);
        }

    }



    /// <summary>
    /// The default geometric converter leverages standard .Net converters to convert gene values to a back from metric space from gene space.
    /// </summary>
    /// <typeparam name="TGeneValue"></typeparam>
    public class DefaultGeometricConverter<TGeneValue> : IGeometricConverter<TGeneValue>
    {
        private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(TGeneValue));


        public bool IsOrdered { get; set; }

        public double GeneToDouble(int geneIndex, TGeneValue geneValue) =>
            Convert.ToDouble(geneValue);


        public TGeneValue DoubleToGene(int geneIndex, double metricValue)
        {
            return (TGeneValue)_converter.ConvertFrom(metricValue);
            //return (TGeneValue) ((IConvertible) metricValue).ToType(typeof(TGeneValue), null);
        }

        public IGeometryEmbedding<TGeneValue> GetEmbedding()
        {
            return null;
        }
    }
}