using System;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// This is a helper class to build an untyped geometry converter from a generic type one
    /// </summary>
    public class TypedGeometricConverter : IGeometricConverter
    {

        private TypedGeometryEmbedding _embedding;

        public void SetTypedConverter<TValue>(IGeometricConverter<TValue> converter)
        {
            ExceptionHelper.ThrowIfNull(nameof(converter), converter);
            GeneToDoubleFunction = (geneIndex, geneValue) => converter.GeneToDouble(geneIndex, (TValue) geneValue);
            DoubleToGeneFunction = (geneIndex, metricValue) => converter.DoubleToGene(geneIndex, metricValue);
            IsOrdered = converter.IsOrdered;
            var embedding = converter.GetEmbedding();
            if (embedding!=null)
            {
                var untypedEmbedding = new TypedGeometryEmbedding();
                untypedEmbedding.SetTypedEmbedding(embedding);
                _embedding = untypedEmbedding;
            }
        }

        public Func<int, object, double> GeneToDoubleFunction { get; set; }

        public Func<int, double, object> DoubleToGeneFunction { get; set; }

        public bool IsOrdered { get; set; }

        public double GeneToDouble(int geneIndex, object geneValue)
        {
            return GeneToDoubleFunction(geneIndex, geneValue);
        }

        public object DoubleToGene(int geneIndex, double metricValue)
        {
            return DoubleToGeneFunction(geneIndex, metricValue);
        }

        public IGeometryEmbedding<object> GetEmbedding()
        {
            return _embedding;
        }
    }
}