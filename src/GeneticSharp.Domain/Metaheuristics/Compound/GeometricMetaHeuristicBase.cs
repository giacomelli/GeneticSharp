using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Reinsertions;

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


        public bool ForceReinsertion { get; set; } = true;

        public IReinsertion CustomReinsertion { get; set; }

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
        public virtual void  SetGeometricConverter<TGeneValue>(IGeometricConverter<TGeneValue> converter)
        {
            var typedNoEmbeddingConverter = new TypedGeometricConverter();
            typedNoEmbeddingConverter.SetTypedConverter(converter);
            GeometricConverter = typedNoEmbeddingConverter;
        }

        public virtual IReinsertion GetDefaultReinsertion()
        {
            return new FitnessBasedElitistReinsertion();
        }

        /// <inheritdoc />
        public IContainerMetaHeuristic Build()
        {
            var toReturn = BuildMainHeuristic();

            //Removing default mutation operator 
            if (NoMutation)
            {
                toReturn.SubMetaHeuristic = new DefaultMetaHeuristic().WithScope(EvolutionStage.Selection | EvolutionStage.Crossover | EvolutionStage.Reinsertion).WithName("No-Mutation MetaHeuristic");
            }

            //Enforcing Pairwise reinsertion
            if (ForceReinsertion)
            {
                var subHeuristic = toReturn.SubMetaHeuristic;
                var reinsertion = CustomReinsertion ?? GetDefaultReinsertion();
                toReturn.SubMetaHeuristic = new ReinsertionHeuristic()
                    { StaticOperator = reinsertion, SubMetaHeuristic = subHeuristic }.WithName($"Forced {reinsertion.GetType().Name} Reinsertion MetaHeuristic");
            }

            return toReturn;

        }

        protected abstract IContainerMetaHeuristic BuildMainHeuristic();


    }
}