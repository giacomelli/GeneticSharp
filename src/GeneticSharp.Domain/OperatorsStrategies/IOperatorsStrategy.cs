using System;
using System.Collections.Generic;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;

namespace GeneticSharp.Domain.OperatorsStrategies
{
    public interface IOperatorsStrategy
    {
        IList<IChromosome> Cross(ICrossover crossover, float crossoverProbability, IList<IChromosome> parents);

        void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes);
    }
}
