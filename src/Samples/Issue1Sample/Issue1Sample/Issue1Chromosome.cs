using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace Issue1Sample
{
	class Issue1Chromosome : ChromosomeBase
	{
		public Issue1Chromosome ()
			: base (2)
		{
			ReplaceGene (0, GenerateGene (0));
			ReplaceGene (1, GenerateGene (1));
		}

		// These properties represents your phenotype.
		public int X { 
			get { 
				return (int)GetGene (0).Value; 
			} 
		}

		public int Y { 
			get { 
				return (int)GetGene (1).Value;
			}
		}

		public override Gene GenerateGene (int geneIndex)
		{
			int value;

			if (geneIndex == 0) {
				value = RandomizationProvider.Current.GetInt (0, 8);
			} else {
				value = RandomizationProvider.Current.GetInt (0, 101);
			}

			return new Gene (value);
		}

		public override IChromosome CreateNew ()
		{
			return new Issue1Chromosome ();
		}

		public override IChromosome Clone ()
		{
			var clone = base.Clone () as Issue1Chromosome;

			return clone;
		}
	}
}
