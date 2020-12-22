using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers.Geometric
{

    /// <summary>
    /// A general interface to define geometry converters/// </summary>
    /// <typeparam name="TValue">The base type of the gene space (typically a .Net value type)</typeparam>
    public interface IGeometricConverter:IGeometricConverter<object>{}
   

    /// <summary>
    /// A general interface to define geometry converters/// </summary>
    /// <typeparam name="TValue">The base type of the gene space (typically a .Net value type)</typeparam>
    public interface IGeometricConverter<TGeneValue>
    {

        bool IsOrdered { get; }

        double GeneToDouble(int geneIndex, TGeneValue geneValue);


        TGeneValue DoubleToGene(int geneIndex, double metricValue);


        IGeometryEmbedding<TGeneValue> GetEmbedding();

    }
}