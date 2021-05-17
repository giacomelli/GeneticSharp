using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics.Compound
{
    /// <summary>
    /// An abstract class for geometric compound metaheuristics with common properties
    /// </summary>
    public abstract class GeometricMetaHeuristicBase: ICompoundMetaheuristic
    {

        /// <summary>
        /// Toggle Mutation Operator (default true switches off mutation operator)
        /// </summary>
        public bool NoMutation { get; set; } = true;


        public bool SetDefaultReinsertion { get; set; } = true;

        /// <summary>
        /// Max expected generations for parameter calibration
        /// </summary>
        public int MaxGenerations { get; set; }


        /// <summary>
        /// A converter providing a gene to/from double converter and an optional geometrisation embedding
        /// </summary>
        public IGeometricConverter GeometricConverter { get; set; }

        /// <summary>
        /// A converter providing a gene to/from double converter and an optional geometrisation embedding
        /// </summary>
        public void  SetGeometricConverter<TGeneValue>(IGeometricConverter<TGeneValue> converter)
        {
            var typedNoEmbeddingConverter = new TypedGeometricConverter();
            typedNoEmbeddingConverter.SetTypedConverter(converter);
            GeometricConverter = typedNoEmbeddingConverter;
        }

        public abstract IContainerMetaHeuristic Build();
    }
}