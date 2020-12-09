using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// The default geometry embedding provides a pass-through for gene values and  
    /// </summary>
    public class OrderedEmbedding<TValue> : IdentityEmbedding<TValue>
    {
        private Func<IChromosome, int, int, bool> _validateSwapFunction;
        public virtual bool IsOrdered { get; set; }

        public GeneSelectionMode GeneSelectionMode { get; set; } = GeneSelectionMode.AllIndexed; 

        /// <summary>
        ///  The default CreateOffspring method is split into 2 cases depending if the chromosome is ordered
        /// </summary>
        /// <param name="parents">the parents producing the offspring</param>
        /// <param name="offSpringValues">the offspring geometric values to convert back to gene values</param>
        public override IChromosome MapFromGeometry(IList<IChromosome> parents, IList<TValue> offSpringValues)
        {
            if (IsOrdered)
            {
                return MapFromGeometryOrdered(parents, offSpringValues);
            }

            return base.MapFromGeometry(parents, offSpringValues);
        }

       

        /// <summary>
        /// The default ordered embedding depends on the  <see cref="GeneSelectionMode"/> property. Depending on this parameter, it runs through some or all the candidate offspring metric-space values considered gene-space values, and with every value to update, it identifies the swap gene index and uses a <see cref="ValidateSwapFunction"/> call to validate or reject the swap. 
        /// </summary>
        public virtual IChromosome MapFromGeometryOrdered(IList<IChromosome> parents, IList<TValue> values)
        {
            var offspring = parents[0].Clone();
            
            IList<int> selectedIndices;
            if ((GeneSelectionMode & GeneSelectionMode.RandomOrder) == GeneSelectionMode.RandomOrder)
            {
                selectedIndices = new List<int>( RandomizationProvider.Current.GetUniqueInts(values.Count, 0, values.Count));
            }
            else
            {
                selectedIndices = Enumerable.Range(0, values.Count).ToList();
            }
            //We precomputed swap indices for easier lookup
            var offspringIndexes = offspring.GetGenes().Select((g,i)=>(g,i)).ToDictionary(gi => (TValue) gi.g.Value, gi=>gi.i);
            foreach (var i in selectedIndices)
            {
                var replacement = (TValue)offspring.GetGene(i).Value;
                if (!replacement.Equals(values[i]))
                {
                    // We find swap index in precomputed index
                    var indexVal = offspringIndexes[values[i]];
                    if (indexVal < 0)
                    {
                        throw new ArgumentException($"value {values[i]} not found in ordered chromosome {offspring}");
                    }
                    if (ValidateSwapFunction(offspring, i, indexVal))
                    {
                        offspring.FlipGene(i, indexVal);
                        //If the gene selection method is FirstChange, we return the corresponding offspring with just one accepted swap
                        if ((GeneSelectionMode & GeneSelectionMode.SingleFirstAllowed) == GeneSelectionMode.SingleFirstAllowed)
                        {
                            return offspring;
                        }
                    }
                }
            }
            return offspring;
        }

        /// <summary>
        /// Targeting ordered chromosomes, A property to hold a swap validation function specific to the problem to solve
        /// </summary>
        public Func<IChromosome, int, int, bool> ValidateSwapFunction
        {
            get
            {
                if (_validateSwapFunction == null)
                {
                    _validateSwapFunction = GetDefaultSwapValidationFunction();
                }
                return _validateSwapFunction;
            }
            set => _validateSwapFunction = value;
        }

        /// <summary>
        /// The default Swap validation function for a kind of problems, can be overwritten with problem specific embeddings
        /// </summary>
        protected  virtual Func<IChromosome, int, int, bool> GetDefaultSwapValidationFunction()
        {
            return (chromosome, indexToSwap1, indexToSwap2) => true;
        }



        



    }




}