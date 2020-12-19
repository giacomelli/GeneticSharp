namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// A general interface to define geometry converters/// </summary>
    /// <typeparam name="TValue">The base type of the gene space (typically a .Net value type)</typeparam>
    public interface IGeometricConverter<TGeneValue>
    {

        double GeneToDouble(int geneIndex, TGeneValue geneValue);


        TGeneValue DoubleToGene(int geneIndex, double metricValue);

    }
}