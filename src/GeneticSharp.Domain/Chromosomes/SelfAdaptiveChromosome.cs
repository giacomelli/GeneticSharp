
using System;
using System.Linq;

namespace GeneticSharp
{
    

    public class SelfAdaptiveChromosome : ChromosomeBase
    {
        public Gene[] GenesValues { get; private set; }
        public double[] MutationProbabilities { get; private set; }

        double _initMutationProvVal;
        public readonly double _minValue, _maxValue;
        
        public SelfAdaptiveChromosome(int length, double minValue = double.MinValue, double maxValue = double.MaxValue, double initMutationProvVal = 0.05)
            : base(length)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _initMutationProvVal = initMutationProvVal;
            GenesValues = new Gene[length];
            MutationProbabilities = new double[length];

            var random = RandomizationProvider.Current;
            for (int i = 0; i < length; i++)
            {
                GenesValues[i] = new Gene(random.GetDouble(minValue, maxValue));
                MutationProbabilities[i] = initMutationProvVal;
                ReplaceGene(i, GenesValues[i]);
            }
        }

        public override IChromosome Clone()
        {
            SelfAdaptiveChromosome c = (SelfAdaptiveChromosome)CreateNew();
            c.ReplaceGenes(0, base.GetGenes());
            c.MutationProbabilities = MutationProbabilities.ToArray();
            return c;
        }

        public override IChromosome CreateNew()
        {
            return new SelfAdaptiveChromosome(GenesValues.Length, _minValue, _maxValue, _initMutationProvVal);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var random = RandomizationProvider.Current;
            double value = random.GetDouble(_minValue, _maxValue);
            GenesValues[geneIndex] = new Gene(value);
            return new Gene(value);
        }

    }
}
