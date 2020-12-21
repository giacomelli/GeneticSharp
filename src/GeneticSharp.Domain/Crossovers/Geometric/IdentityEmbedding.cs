using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// The default geometry embedding provides a pass-through for gene values and  geometric values
    /// </summary>
    public class IdentityEmbedding<TValue> : IGeometryEmbedding<TValue>
    {

        /// <summary>
        ///  The default CreateOffspring method is split into 2 cases depending if the chromosome is ordered
        /// </summary>
        /// <param name="parents">the parents producing the offspring</param>
        /// <param name="offSpringValues">the offspring geometric values to convert back to gene values</param>
        public virtual IChromosome MapFromGeometry(IList<IChromosome> parents, IList<TValue> offSpringValues)
        {
            return IdentityMapFromGeometry(parents, offSpringValues);
        }

        /// <summary>
        /// The default unordered embedding uses the default pass-through <see cref="IdentityMapToGeometry"/> method.
        /// </summary>
        public virtual IList<TValue> MapToGeometry(IChromosome parent)
        {
            return IdentityMapToGeometry(parent);
        }

        /// <summary>
        /// The default Offspring creation method is a pass through between metric values and gene values
        /// </summary>
        public static IChromosome IdentityMapFromGeometry(IList<IChromosome> parent, IList<TValue> values)
        {
            var offspring = parent.First().CreateNew();

            var i = 0;
            foreach (var value in values)
            {
                offspring.ReplaceGene(i, new Gene(value));
                i++;
            }

            return offspring;
        }


        /// <summary>
        /// The default parent creation method is a pass through between gene values and geometric values
        /// </summary>
        public static IList<TValue> IdentityMapToGeometry(IChromosome parent)
        {
            return parent.GetGenes().Select(g => (TValue)g.Value).ToList();
        }

    }
}