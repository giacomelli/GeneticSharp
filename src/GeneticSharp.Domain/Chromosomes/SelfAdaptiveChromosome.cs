using GeneticSharp;


namespace GeneticSharp
{

    public class SelfAdaptiveChromosome : ChromosomeBase
    {
        public Gene[] GenesValues { get; private set; }
        public double[] MutationProbabilities { get; private set; }
        public readonly double _minValue, _maxValue;

        public SelfAdaptiveChromosome(int length, double minValue = double.MinValue, double maxValue = double.MaxValue, double initMutationProvVal = 0.05)
            : base(length)
        {
            _minValue = minValue;
            _maxValue = maxValue;
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

        public override IChromosome CreateNew()
        {
            return new SelfAdaptiveChromosome(GenesValues.Length, _minValue, _maxValue);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var random = RandomizationProvider.Current;
            // Genera un valor aleatorio dentro del rango definido.
            double value = random.GetDouble(_minValue, _maxValue);
            GenesValues[geneIndex] = new Gene(value);
            return new Gene(value);
        }
    }
}