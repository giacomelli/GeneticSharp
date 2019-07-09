using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

public class TspChromosome : ChromosomeBase
{
    private readonly int _numberOfCities;

    public TspChromosome(int numberOfCities) : base(numberOfCities)
    {
        _numberOfCities = numberOfCities;
        var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(numberOfCities, 0, numberOfCities);

        for (int i = 0; i < numberOfCities; i++)
        {
            ReplaceGene(i, new Gene(citiesIndexes[i]));
        }
    }

    public double Distance { get; internal set; }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(RandomizationProvider.Current.GetInt(0, _numberOfCities));
    }

    public override IChromosome CreateNew()
    {
        return new TspChromosome(_numberOfCities);
    }

    public override IChromosome Clone()
    {
        var clone = base.Clone() as TspChromosome;
        clone.Distance = Distance;

        return clone;
    }
}