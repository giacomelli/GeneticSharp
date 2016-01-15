using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Extensions.AutoConfig
{
    /// <summary>
    /// Auto config another genetic algorithm.
    /// </summary>
    public sealed class AutoConfigChromosome : ChromosomeBase
    {
        #region Fields
        private static IRandomization s_randomization = RandomizationProvider.Current;
        private static IList<string> s_availableSelections = SelectionService.GetSelectionNames();
        private static IList<string> s_availableCrossovers = CrossoverService.GetCrossoverNames();
        private static IList<string> s_availableMutations = MutationService.GetMutationNames();
        #endregion

        #region Constructor           
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoConfigChromosome"/> class.
        /// </summary>
        public AutoConfigChromosome() : base(3)
        {
            CreateGenes();
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <value>
        /// The selection.
        /// </value>
        public ISelection Selection
        {
            get
            {
                return GetGene(0).Value as ISelection;
            }
        }

        /// <summary>
        /// Gets the crossover.
        /// </summary>
        /// <value>
        /// The crossover.
        /// </value>
        public ICrossover Crossover
        {
            get
            {
                return GetGene(1).Value as ICrossover;
            }
        }

        /// <summary>
        /// Gets the mutation.
        /// </summary>
        /// <value>
        /// The mutation.
        /// </value>
        public IMutation Mutation
        {
            get
            {
                return GetGene(2).Value as IMutation;
            }
        }
        #endregion

        #region Methods                
        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>
        /// The new chromosome.
        /// </returns>
        public override IChromosome CreateNew()
        {
            return new AutoConfigChromosome();
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns>The new gene.</returns>
        /// <exception cref="System.InvalidOperationException">Invalid AutoConfigChromosome gene index.</exception>
        public override Gene GenerateGene(int geneIndex)
        {
            switch (geneIndex)
            {
                // Selection.
                case 0:
                    return CreateRandomGene<ISelection>(s_availableSelections);

                // Crossover.
                case 1:
                    return CreateRandomGene<ICrossover>(s_availableCrossovers);

                // Mutation.
                case 2:
                    return CreateRandomGene<IMutation>(s_availableMutations);

                default:
                    throw new InvalidOperationException("Invalid AutoConfigChromosome gene index.");
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        private static Gene CreateRandomGene<TGeneValue>(IList<string> available)
        {
            return new Gene(TypeHelper.CreateInstanceByName<TGeneValue>(available[s_randomization.GetInt(0, available.Count)]));
        }
        #endregion
    }
}
