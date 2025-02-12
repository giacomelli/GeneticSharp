using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GeneticSharp.Domain.LifeSpans
{
    [DisplayName("LifespanReinsertion")]
    public class LifespanReinsertionDecorator: IReinsertion
    {
        IReinsertion _baseReinsertion;

        public int MaxLifespan { get; set; }

        public bool CanCollapse => _baseReinsertion.CanCollapse;

        public bool CanExpand => _baseReinsertion.CanExpand;

        public LifespanReinsertionDecorator(IReinsertion baseReinsertion, int maxLifespan = 20)
        {
            _baseReinsertion = baseReinsertion;
            MaxLifespan = maxLifespan;
        }


        public IList<IChromosome> SelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            parents = parents.Where(r => r.Age < MaxLifespan).ToList();
            offspring = offspring.Where(r => r.Age < MaxLifespan).ToList();
            offspring =  _baseReinsertion.SelectChromosomes(population, offspring, parents);
            foreach (var e in offspring)
                e.Age++;
            return offspring;
        }
    }
}
