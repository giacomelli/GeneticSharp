using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.Metaheuristics
{

    /// <summary>
    /// A MetaGenetic Algorithm uses advanced MetaHeursitics to control evolution stages dynamically through dynamic parameters, expressions and rules.
    /// </summary>
    public class MetaGeneticAlgorithm : GeneticAlgorithm
    {

        /// <summary>
        /// Gets or sets the Metaheuristic operator.
        /// </summary>
        public IMetaHeuristic Metaheuristic { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Metaheuristics.MetaGeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="population">The chromosomes population.</param>
        /// <param name="fitness">The fitness evaluation function.</param>
        /// <param name="selection">The selection operator.</param>
        /// <param name="crossover">The crossover operator.</param>
        /// <param name="mutation">The mutation operator.</param>
        public MetaGeneticAlgorithm(
            IPopulation population,
            IFitness fitness,
            ISelection selection,
            ICrossover crossover,
            IMutation mutation):base(population,fitness,selection, crossover, mutation)
        {
            Metaheuristic = new DefaultMetaHeuristic();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Metaheuristics.MetaGeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="population">The chromosomes population.</param>
        /// <param name="fitness">The fitness evaluation function.</param>
        /// <param name="selection">The selection operator.</param>
        /// <param name="crossover">The crossover operator.</param>
        /// <param name="mutation">The mutation operator.</param>
        /// <param name="metaHeuristic">the Metaheuristic</param>
        public MetaGeneticAlgorithm(
            IPopulation population,
            IFitness fitness,
            ISelection selection,
            ICrossover crossover,
            IMutation mutation,
            IMetaHeuristic metaHeuristic) : base(population, fitness, selection, crossover, mutation)
        {
            Metaheuristic = metaHeuristic;
        }

        protected override void EvolveOneGeneration()
        {
            var ctx = Metaheuristic.GetContext(this, Population);
            ctx.CurrentStage = EvolutionStage.Selection;
            var parents = SelectParents(ctx);
            ctx.CurrentStage = EvolutionStage.Crossover;
            var offspring = Cross(ctx, parents);
            ctx.CurrentStage = EvolutionStage.Mutation;
            Mutate(ctx, offspring);

            EvaluateFitness(offspring);

            ctx.CurrentStage = EvolutionStage.Reinsertion;
            var newGenerationChromosomes = Reinsert(ctx, offspring, parents);
            Population.CreateNewGeneration(newGenerationChromosomes);
        }


        /// <summary>
        /// Selects the parents.
        /// </summary>
        /// <returns>The parents.</returns>
        private IList<IChromosome> SelectParents(IEvolutionContext ctx)
        {
            return Metaheuristic.SelectParentPopulation(ctx, Selection);

        }

        /// <summary>
        /// Crosses the specified parents.
        /// </summary>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The result chromosomes.</returns>
        private IList<IChromosome> Cross(IEvolutionContext ctx, IList<IChromosome> parents)
        {
            return OperatorsStrategy.MetaCross(Metaheuristic, ctx, Crossover, CrossoverProbability, parents);
        }

        /// <summary>
        /// Mutate the specified chromosomes.
        /// </summary>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="chromosomes">The chromosomes.</param>
        private void Mutate(IEvolutionContext ctx, IList<IChromosome> chromosomes)
        {
            OperatorsStrategy.MetaMutate(Metaheuristic, ctx, Mutation, MutationProbability, chromosomes);
        }

        /// <summary>
        /// Reinsert the specified offspring and parents.
        /// </summary>
        /// <param name="ctx">the current evolution context</param>
        /// <param name="offspring">The offspring chromosomes.</param>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The reinserted chromosomes.
        /// </returns>
        private IList<IChromosome> Reinsert(IEvolutionContext ctx, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            return Metaheuristic.Reinsert(ctx, Reinsertion, offspring, parents);
        }


    }
}