using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp
{
    public class SelfAdaptiveChromosome : ChromosomeBase
    {
        Dictionary<int, double> _mutationProbabilities;
        int _length;
        double _initMutationProvVal;
        public readonly double _minValue, _maxValue;

        public SelfAdaptiveChromosome(int length, double minValue = double.MinValue, double maxValue = double.MaxValue, double initMutationProvVal = 0.05)
            : base(length)
        {
            _length = length;
            _minValue = minValue;
            _maxValue = maxValue;
            _initMutationProvVal = initMutationProvVal;
            _mutationProbabilities = new Dictionary<int, double>();

            var random = RandomizationProvider.Current;
            for (int i = 0; i < length; i++)
            {
                var g = new Gene(random.GetDouble(minValue, maxValue));
                base.ReplaceGene(i, g);
            }
        }

        public double GetMutationProbability(int index)
        {
            double d;
            if (!_mutationProbabilities.TryGetValue(index, out d))
                return _initMutationProvVal;
            return d;
        }

        public void SetMutationProbability(int index, double prov)
        {
            if(prov != _initMutationProvVal)
                _mutationProbabilities[index] = prov;
        }

        public override IChromosome Clone()
        {
            SelfAdaptiveChromosome c = (SelfAdaptiveChromosome)base.Clone();
            c._mutationProbabilities = _mutationProbabilities.ToDictionary(r => r.Key, r => r.Value);
            return c;
        }


        public override IChromosome CreateNew()
        {
            var e = new SelfAdaptiveChromosome(_length, _minValue, _maxValue, _initMutationProvVal);
            return e;
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var random = RandomizationProvider.Current;
            double value = random.GetDouble(_minValue, _maxValue);
            var g = new Gene(value);
            base.ReplaceGene(geneIndex, g);
            return new Gene(value);
        }
    }
}