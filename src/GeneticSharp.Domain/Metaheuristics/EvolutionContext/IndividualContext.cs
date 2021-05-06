using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Metaheuristics
{



    public class IndividualContext : SubEvolutionContext
    {

        public IndividualContext(IEvolutionContext populationContext, int originalIndex, int localIndex):base(populationContext)
        {
            OriginalIndex = originalIndex;
            LocalIndex = localIndex;
        }

        /// <inheritdoc />
        public override IEvolutionContext GetIndividual(int index)
        {
            //todo:figure out if this is a problem with nesting
            if (index != OriginalIndex)
            {
                return PopulationContext.GetIndividual(index);
            }
            return this;
        }

        public override int OriginalIndex { get; }
        public override int LocalIndex { get; }

        private IList<IChromosome> _selectedParents;

        public override IList<IChromosome> SelectedParents
        {
            get {
                if (_selectedParents!=null)
                {
                    return _selectedParents;
                }

                return base.SelectedParents;
            }
            set => _selectedParents = value;
        }
    }
}