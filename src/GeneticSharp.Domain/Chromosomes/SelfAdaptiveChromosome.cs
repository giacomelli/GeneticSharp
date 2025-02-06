
using System;

namespace GeneticSharp
{
    public enum CrossoverType
    {
        OnePoin,
        TwoPoints,
        Uniform,
    }

    public enum MutationType
    {
        Simple,
        WithImprovement,
        Strong,
    }

    public class SelfAdaptiveChromosome : ChromosomeBase
    {
        public double MutationProbability { get; private set; }
        public Gene[] GenesValues { get; private set; }
        public double[] MutationProbabilities { get; private set; }
        public CrossoverType CrossoverType { get; private set; } 
        public MutationType MutationType { get; private set; }  

        public readonly double _minValue, _maxValue;
        private static readonly int CrossoverOperatorsCount = 3;
        

        public SelfAdaptiveChromosome(int length, double minValue = double.MinValue, double maxValue = double.MaxValue, double initMutationProvVal = 0.05, double mutationProbability = 0.1)
            : base(length)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            GenesValues = new Gene[length];
            MutationProbabilities = new double[length];
            MutationProbability = mutationProbability;

            var random = RandomizationProvider.Current;
            for (int i = 0; i < length; i++)
            {
                GenesValues[i] = new Gene(random.GetDouble(minValue, maxValue));
                MutationProbabilities[i] = initMutationProvVal;
                ReplaceGene(i, GenesValues[i]);
            }

            CrossoverType = (CrossoverType)random.GetInt(0, Enum.GetValues(typeof(CrossoverType)).Length);
            MutationType = (MutationType)random.GetInt(0, Enum.GetValues(typeof(MutationType)).Length);
        }

        public override IChromosome CreateNew()
        {
            return new SelfAdaptiveChromosome(GenesValues.Length, _minValue, _maxValue);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var random = RandomizationProvider.Current;
            double value = random.GetDouble(_minValue, _maxValue);
            GenesValues[geneIndex] = new Gene(value);
            return new Gene(value);
        }

        public void MutateOperators()
        {
            var random = RandomizationProvider.Current;
            if (random.GetDouble() < MutationProbability)
                CrossoverType = (CrossoverType)random.GetInt(0, Enum.GetValues(typeof(CrossoverType)).Length);

            if (random.GetDouble() < MutationProbability)  
                MutationType = (MutationType)random.GetInt(0, Enum.GetValues(typeof(MutationType)).Length);
        }
    }
}
