using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using System.ComponentModel;

namespace GeneticSharp.Domain.Crossovers {
    /// <summary>
    /// The Uniform Crossover uses a fixed mixing ratio between two parents. 
    /// Unlike one-point and two-point crossover, the Uniform Crossover enables the parent chromosomes to contribute the gene level rather than the segment level.
    /// <remarks>
    /// If the mix probability is 0.5, the offspring has approximately half of the genes from first parent and the other half from second parent, although cross over points can be randomly chosen.
    /// </remarks>
    /// <see href="http://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)#Uniform_Crossover_and_Half_Uniform_Crossover">Wikipedia</see>
    /// </summary>
    [DisplayName("Uniform")]
    public class UniformCrossover : CrossoverBase {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UniformCrossover"/> class.
        /// </summary>
        /// <param name="mixProbability">The mix probability. The default mix probability is 0.5.</param>
        /// <param name="childrenNumber">How many children to generate of two parents.</param>
        /// <param name="counterChild">Whether to generate the counter children.</param>
        public UniformCrossover(float mixProbability = 0.5f, int childrenNumber = 2, bool counterChild = true) : base(2, childrenNumber) {
            MixProbability = mixProbability;
            CounterChild = counterChild;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mix probability.
        /// </summary>
        /// <value>The mix probability.</value>
        public float MixProbability { get; set; }

        /// <summary>
        /// Gets or sets whether to generate the counter children.
        /// </summary>
        /// <value>Whether to generate the counter children</value>
        public bool CounterChild { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents) {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var offspring = new List<IChromosome>(ChildrenNumber);

            while (offspring.Count < ChildrenNumber) {
                var firstChild = firstParent.CreateNew();
                var secondChild = secondParent.CreateNew();
                for (int i = 0; i < firstParent.Length; i++) {
                    if (RandomizationProvider.Current.GetDouble() < MixProbability) {
                        firstChild.ReplaceGene(i, firstParent.GetGene(i));
                        secondChild.ReplaceGene(i, secondParent.GetGene(i));
                    } else {
                        firstChild.ReplaceGene(i, secondParent.GetGene(i));
                        secondChild.ReplaceGene(i, firstParent.GetGene(i));
                    }
                }

                offspring.Add(firstChild);
                if (CounterChild && offspring.Count < ChildrenNumber) offspring.Add(secondChild);
            }
            
            return offspring;
        }
        #endregion
    }
}
