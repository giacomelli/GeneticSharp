using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Metaheuristics;

namespace GeneticSharp.Domain.Populations
{
    /// <summary>
    /// Sub population serves applying genetic operators to population Islands or of Eurkaryote Subchromosomes
    /// </summary>
    public class SubPopulation : Population
    {
        public IPopulation ParentPopulation { get; set; }

       

        public SubPopulation(IPopulation parentPopulation, IList<IChromosome> subPopulation) : base(parentPopulation.MinSize, parentPopulation.MaxSize, subPopulation[0])
        {
            ParentPopulation = parentPopulation;
            CreateNewGeneration(subPopulation);
            GenerationsNumber = parentPopulation.GenerationsNumber;
            MinSize = subPopulation.Count;
            MaxSize = (parentPopulation.MaxSize / parentPopulation.MinSize) * MinSize;
            EndCurrentGeneration();
        }

        private IEvolutionContext _subContext;

        public IEvolutionContext GetContext(IEvolutionContext parentContext)
        {
            if (_subContext == null)
            {
                lock (this)
                {
                    if (_subContext == null)
                    {
                        _subContext = new SubPopulationContext(parentContext, this);
                    }
                }
            }

            var toReturn = _subContext;
            if (parentContext.OriginalIndex!=toReturn.OriginalIndex)
            {
                toReturn = toReturn.GetIndividual(parentContext.OriginalIndex);
            }
            if (parentContext.LocalIndex != toReturn.LocalIndex)
            {
                toReturn = toReturn.GetLocal(parentContext.LocalIndex);
            }

            return toReturn;
        }
    }
}