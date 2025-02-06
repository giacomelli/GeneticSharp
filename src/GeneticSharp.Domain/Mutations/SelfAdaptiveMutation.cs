

using System;

namespace GeneticSharp
{
    public class SelfAdaptiveMutation : MutationBase
    {
        // Constantes para la auto-adaptación
        private readonly double tau;
        private readonly double sigmaMutationProbability;

        public SelfAdaptiveMutation(double tau = 0.1, double sigmaMutationProbability = 0.05)
        {
            this.tau = tau;
            this.sigmaMutationProbability = sigmaMutationProbability;
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            var adaptiveChromosome = chromosome as SelfAdaptiveChromosome;
            if (adaptiveChromosome == null)
            {
                throw new ArgumentException("The chromosome must be of type AdaptiveChromosome.", nameof(chromosome));
            }

            var random = RandomizationProvider.Current;

            for (int i = 0; i < adaptiveChromosome.GenesValues.Length; i++)
            {
                // Actualizar la probabilidad de mutación usando una mutación auto-adaptativa
                if (random.GetDouble() < sigmaMutationProbability)
                {
                    double normal = NextGaussian(0, 1);
                    adaptiveChromosome.MutationProbabilities[i] *= Math.Exp(tau * normal);
                    adaptiveChromosome.MutationProbabilities[i] = Math.Min(1.0, Math.Max(0, adaptiveChromosome.MutationProbabilities[i]));
                }

                // Mutar el gen en función de su probabilidad individual
                if (random.GetDouble() < adaptiveChromosome.MutationProbabilities[i])
                {
                    // Ejemplo de mutación: sumar un valor aleatorio pequeño al gen actual.
                    double currentValue = (double)adaptiveChromosome.GenesValues[i].Value;
                    double mutatedValue = currentValue + random.GetDouble(-1, 1); // Ajusta el rango según convenga
                                                                                  // Aseguramos que el nuevo valor esté dentro de los límites
                    mutatedValue = Math.Min(adaptiveChromosome._maxValue, Math.Max(adaptiveChromosome._minValue, mutatedValue));
                    adaptiveChromosome.GenesValues[i] = new Gene(mutatedValue);
                    adaptiveChromosome.ReplaceGene(i, new Gene(mutatedValue));
                }
            }
        }

        // Método para generar un número aleatorio con distribución normal (transformada Box-Muller)
        private double NextGaussian(double mean, double stdDev)
        {
            var random = RandomizationProvider.Current;
            double u1 = random.GetDouble();
            double u2 = random.GetDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}